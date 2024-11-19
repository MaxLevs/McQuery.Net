using McQuery.Net.Data;

namespace McQuery.Net.Internal.Parsers;

internal class BasicStatusResponseParser : StatusResponseParser<BasicStatus>
{
    public override BasicStatus Parse(byte[] data)
    {
        var sessionId = StartParsing(data, out var reader);

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
            SessionId = sessionId,
        };
    }
}
