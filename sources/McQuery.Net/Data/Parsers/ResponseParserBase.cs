using System.Buffers;
using System.Text;

namespace McQuery.Net.Data.Parsers;

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
