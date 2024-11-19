using McQuery.Net.Data;

namespace McQuery.Net.Internal.Parsers;

internal class FullStatusResponseParser : StatusResponseParser<FullStatus>
{
    private static readonly byte[] constant1 = [0x73, 0x70, 0x6C, 0x69, 0x74, 0x6E, 0x75, 0x6D, 0x00, 0x80, 0x00];
    private static readonly byte[] constant2 = [0x01, 0x70, 0x6C, 0x61, 0x79, 0x65, 0x72, 0x5F, 0x00, 0x00];
    private static readonly InvalidOperationException responseFormatError = new("Invalid full status response format");

    public override FullStatus Parse(byte[] data)
    {
        var sessionId = StartParsing(data, out var reader);

        if (!reader.IsNext(constant1, advancePast: true)) throw responseFormatError;

        Dictionary<string, string> statusKeyValues = new();
        while (!reader.IsNext(next: 0, advancePast: true))
        {
            var key = ParseNullTerminatingString(ref reader);
            var value = ParseNullTerminatingString(ref reader);
            statusKeyValues.Add(key, value);
        }

        if (!reader.IsNext(constant2, advancePast: true)) throw responseFormatError;

        List<string> players = [];
        while (!reader.IsNext(next: 0, advancePast: true))
        {
            players.Add(ParseNullTerminatingString(ref reader));
        }

        return new FullStatus(
            statusKeyValues["hostname"],
            statusKeyValues["gametype"],
            statusKeyValues["game_id"],
            statusKeyValues["version"],
            statusKeyValues["plugins"],
            statusKeyValues["map"],
            int.Parse(statusKeyValues["numplayers"]),
            int.Parse(statusKeyValues["maxplayers"]),
            players.ToArray(),
            int.Parse(statusKeyValues["hostport"]),
            statusKeyValues["hostip"])
        {
            SessionId = sessionId,
        };
    }
}