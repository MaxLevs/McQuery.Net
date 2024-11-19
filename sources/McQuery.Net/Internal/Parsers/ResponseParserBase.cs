using System.Buffers;
using System.Text;
using McQuery.Net.Internal.Data;

namespace McQuery.Net.Internal.Parsers;

internal abstract class ResponseParserBase
{
    protected abstract byte ResponseType { get; }

    internal SessionId StartParsing(byte[] data, out SequenceReader<byte> reader)
    {
        ReadOnlySequence<byte> sequence = new(data);
        reader = new SequenceReader<byte>(sequence);

        if (!reader.IsNext([ResponseType], advancePast: true)) throw new InvalidOperationException("Invalid response type");

        return ParseSessionId(ref reader);
    }

    private static SessionId ParseSessionId(ref SequenceReader<byte> reader)
    {
        if (reader.UnreadSequence.Length < 4)
        {
            throw new InvalidOperationException("Session id must contain exactly 4 bytes.");
        }

        reader.TryReadExact(count: 4, out var sessionIdBytes);

        return new SessionId(sessionIdBytes.ToArray());
    }

    internal static string ParseNullTerminatingString(ref SequenceReader<byte> reader)
    {
        if (!reader.TryReadTo(out ReadOnlySequence<byte> bytes, delimiter: 0, advancePastDelimiter: true))
        {
            throw new InvalidOperationException("Cannot parse null terminating string: terminator was not found.");
        }

        return Encoding.ASCII.GetString(bytes);
    }

    internal static short ParseShortLittleEndian(ref SequenceReader<byte> reader)
    {
        if (!reader.TryReadLittleEndian(out short port))
        {
            throw new InvalidOperationException("Cannot parse short value");
        }

        return port;
    }
}
