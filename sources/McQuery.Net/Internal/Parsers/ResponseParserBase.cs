using System.Buffers;
using System.Text;
using McQuery.Net.Exceptions;
using McQuery.Net.Internal.Data;
using Microsoft.Extensions.Logging;

namespace McQuery.Net.Internal.Parsers;

internal abstract class ResponseParserBase
{
    protected readonly ILogger Logger;

    protected ResponseParserBase(ILogger logger) =>
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));

    protected abstract byte ResponseType { get; }

    internal SessionId StartParsing(byte[] data, out SequenceReader<byte> reader)
    {
        ReadOnlySequence<byte> sequence = new(data);
        reader = new SequenceReader<byte>(sequence);

        if (!reader.IsNext([ResponseType], advancePast: true))
        {
            Logger.LogError("Invalid response type, expected {Type}: [{Data}]", ResponseType, BitConverter.ToString(data));
            throw new McQueryResponseParsingException("Invalid response type");
        }

        return ParseSessionId(ref reader);
    }

    private static SessionId ParseSessionId(ref SequenceReader<byte> reader)
    {
        if (!reader.TryReadExact(count: 4, out var sessionIdBytes))
        {
            throw new McQueryResponseParsingException("Session id must contain exactly 4 bytes.");
        }

        return new SessionId(sessionIdBytes.ToArray());
    }

    internal static string ParseNullTerminatingString(ref SequenceReader<byte> reader)
    {
        if (!reader.TryReadTo(out ReadOnlySequence<byte> bytes, delimiter: 0, advancePastDelimiter: true))
        {
            throw new McQueryResponseParsingException("Cannot parse null terminating string: terminator was not found.");
        }

        return Encoding.ASCII.GetString(bytes);
    }

    internal static short ParseShortLittleEndian(ref SequenceReader<byte> reader)
    {
        if (!reader.TryReadLittleEndian(out short port))
        {
            throw new McQueryResponseParsingException("Cannot parse short value");
        }

        return port;
    }
}
