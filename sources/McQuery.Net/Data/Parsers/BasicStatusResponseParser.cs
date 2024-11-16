using System.Buffers;
using McQuery.Net.Data.Responses;

namespace McQuery.Net.Data.Parsers;

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
