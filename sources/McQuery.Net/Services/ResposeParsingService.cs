using System.Buffers;
using System.Text;
using McQuery.Net.Data;
using McQuery.Net.Data.Packages.Responses;

namespace McQuery.Net.Services;

/// <summary>
/// This class parses Minecraft Query response packages for getting data from it
/// Wiki: https://wiki.vg/Query
/// </summary>
public static class ResposeParsingService
{
    public static byte ParseType(byte[] data) => data[0];

    public static SessionId ParseSessionId(ref SequenceReader<byte> reader)
    {
        if (reader.UnreadSequence.Length < 4) throw new IncorrectPackageDataException(reader.Sequence.ToArray());
        byte[]? sessionIdBytes = new byte[4];
        Span<byte> sessionIdSpan = new(sessionIdBytes);
        reader.TryCopyTo(sessionIdSpan);
        reader.Advance(4);

        return new SessionId(sessionIdSpan.ToArray());
    }

    /// <summary>
    /// Parses response package and returns ChallengeToken
    /// </summary>
    /// <param name="rawResponse">RawResponce package</param>
    /// <returns>byte[] array which contains ChallengeToken as big-endian</returns>
    public static byte[] ParseHandshake(RawResponse rawResponse)
    {
        byte[]? data = (byte[])rawResponse.RawData.Clone();

        if (data.Length < 5) throw new IncorrectPackageDataException(data);
        byte[]? response = BitConverter.GetBytes(int.Parse(Encoding.ASCII.GetString(data, 5, rawResponse.RawData.Length - 6)));
        if (BitConverter.IsLittleEndian) response = response.Reverse().ToArray();

        return response;
    }

    public static ServerBasicStateResponse ParseBasicState(RawResponse rawResponse)
    {
        if (rawResponse.RawData.Length <= 5)
            throw new IncorrectPackageDataException(rawResponse.RawData);

        SequenceReader<byte> reader = new(new ReadOnlySequence<byte>(rawResponse.RawData));
        reader.Advance(1); // Skip Type

        SessionId? sessionId = ParseSessionId(ref reader);

        string? motd = ReadString(ref reader);
        string? gameType = ReadString(ref reader);
        string? map = ReadString(ref reader);
        int numPlayers = int.Parse(ReadString(ref reader));
        int maxPlayers = int.Parse(ReadString(ref reader));

        if (!reader.TryReadLittleEndian(out short port))
            throw new IncorrectPackageDataException(rawResponse.RawData);

        string? hostIp = ReadString(ref reader);

        ServerBasicStateResponse serverInfo = new(
            rawResponse.ServerUUID,
            sessionId,
            motd,
            gameType,
            map,
            numPlayers,
            maxPlayers,
            port,
            hostIp,
            (byte[])rawResponse.RawData.Clone()
        );

        return serverInfo;
    }

    private static readonly byte[] constant1 = new byte[] { 0x73, 0x70, 0x6C, 0x69, 0x74, 0x6E, 0x75, 0x6D, 0x00, 0x80, 0x00 };

    private static readonly byte[] constant2 = new byte[] { 0x01, 0x70, 0x6C, 0x61, 0x79, 0x65, 0x72, 0x5F, 0x00, 0x00 };

    public static ServerFullStateResponse ParseFullState(RawResponse rawResponse)
    {
        if (rawResponse.RawData.Length <= 5)
            throw new IncorrectPackageDataException(rawResponse.RawData);

        SequenceReader<byte> reader = new(new ReadOnlySequence<byte>(rawResponse.RawData));
        reader.Advance(1); // Skip Type

        SessionId? sessionId = ParseSessionId(ref reader);

        if (!reader.IsNext(constant1, true))
            throw new IncorrectPackageDataException(rawResponse.RawData);

        Dictionary<string, string>? statusKeyValues = new();
        while (!reader.IsNext(0, true))
        {
            string? key = ReadString(ref reader);
            string? value = ReadString(ref reader);
            statusKeyValues.Add(key, value);
        }

        if (!reader.IsNext(constant2, true)) // Padding: 10 bytes constant
            throw new IncorrectPackageDataException(rawResponse.RawData);

        List<string>? players = new();
        while (!reader.IsNext(0, true)) players.Add(ReadString(ref reader));

        ServerFullStateResponse fullState = new
        (
            rawResponse.ServerUUID,
            sessionId,
            statusKeyValues["hostname"],
            statusKeyValues["gametype"],
            statusKeyValues["game_id"],
            statusKeyValues["version"],
            statusKeyValues["plugins"],
            statusKeyValues["map"],
            int.Parse(statusKeyValues["numplayers"]),
            int.Parse(statusKeyValues["maxplayers"]),
            players.ToArray(),
            hostIp: statusKeyValues["hostip"],
            hostPort: int.Parse(statusKeyValues["hostport"]),
            rawData: (byte[])rawResponse.RawData.Clone()
        );

        return fullState;
    }

    private static string ReadString(ref SequenceReader<byte> reader)
    {
        if (!reader.TryReadTo(out ReadOnlySequence<byte> bytes, 0, true))
            throw new IncorrectPackageDataException("Zero byte not found", reader.Sequence.ToArray());

        return Encoding.ASCII.GetString(bytes); // а точно ASCII? Может, Utf8?
    }
}

public class IncorrectPackageDataException : Exception
{
    public byte[] data { get; }

    public IncorrectPackageDataException(byte[] data)
    {
        this.data = data;
    }

    public IncorrectPackageDataException(string? message, byte[] data) : base(message)
    {
        this.data = data;
    }

    public IncorrectPackageDataException(string? message, Exception? innerException, byte[] data) : base(message, innerException)
    {
        this.data = data;
    }
}
