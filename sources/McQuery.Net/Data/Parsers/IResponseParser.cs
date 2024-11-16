using System.Buffers;
using System.Text;
using McQuery.Net.Data.Responses;

namespace McQuery.Net.Data.Parsers;

internal interface IResponseParser<out T>
{
    T Parse(byte[] data);
}

internal abstract class ResponseParserBase
{
    public abstract byte ResponseType { get; }

    public SessionId StartParsing(byte[] data, out SequenceReader<byte> reader)
    {
        ReadOnlySequence<byte> sequence = new(data);
        reader = new SequenceReader<byte>(sequence);

        if (!reader.IsNext([ResponseType], true))
        {
            throw new InvalidOperationException("Invalid response type");
        }

        return ParseSessionId(ref reader);
    }

    private static SessionId ParseSessionId(ref SequenceReader<byte> reader)
    {
        if (reader.UnreadSequence.Length < 4)
        {
            throw new InvalidOperationException("Session id must contain exactly 4 bytes.");
        }

        reader.TryReadExact(4, out ReadOnlySequence<byte> sessionIdBytes);

        return new SessionId(sessionIdBytes.ToArray());
    }

    internal static string ParseNullTerminatingString(ref SequenceReader<byte> reader)
    {
        if (!reader.TryReadTo(out ReadOnlySequence<byte> bytes, 0, true))
            throw new InvalidOperationException("Cannot parse null terminating string: terminator was not found.");

        return Encoding.ASCII.GetString(bytes);
    }

    internal static short ParseShortLittleEndian(ref SequenceReader<byte> reader)
    {
        if (!reader.TryReadLittleEndian(out short port))
            throw new InvalidOperationException("Cannot parse short value");

        return port;
    }
}

internal class HandshakeResponseParser : ResponseParserBase, IResponseParser<ChallengeToken>
{
    public override byte ResponseType => 0x09;

    public ChallengeToken Parse(byte[] data)
    {
        StartParsing(data, out SequenceReader<byte> reader);

        string challengeTokenString = ParseNullTerminatingString(ref reader);
        byte[] challengeTokenBytes = BitConverter.GetBytes(int.Parse(challengeTokenString));

        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(challengeTokenBytes);
        }

        return new ChallengeToken(challengeTokenBytes);
    }
}

internal abstract class StatusResponseParser<T> : ResponseParserBase, IResponseParser<T> where T : StatusBase
{
    public override byte ResponseType => 0x00;

    public abstract T Parse(byte[] data);
}

internal class BasicStatusResponseParser : StatusResponseParser<BasicStatus>
{
    public override BasicStatus Parse(byte[] data)
    {
        SessionId sessionId = StartParsing(data, out SequenceReader<byte> reader);

        return new BasicStatus(
            ParseNullTerminatingString(ref reader),
            ParseNullTerminatingString(ref reader),
            ParseNullTerminatingString(ref reader),
            int.Parse(ParseNullTerminatingString(ref reader)),
            int.Parse(ParseNullTerminatingString(ref reader)),
            ParseShortLittleEndian(ref reader),
            ParseNullTerminatingString(ref reader)
        )
        {
            SessionId = sessionId
        };
    }
}
