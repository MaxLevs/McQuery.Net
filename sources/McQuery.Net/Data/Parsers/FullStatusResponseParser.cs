using System.Buffers;
using McQuery.Net.Data.Responses;

namespace McQuery.Net.Data.Parsers;

internal class FullStatusResponseParser : StatusResponseParser<FullStatus>
{
    private static readonly byte[] Constant1 = [0x73, 0x70, 0x6C, 0x69, 0x74, 0x6E, 0x75, 0x6D, 0x00, 0x80, 0x00];

    private static readonly byte[] Constant2 = [0x01, 0x70, 0x6C, 0x61, 0x79, 0x65, 0x72, 0x5F, 0x00, 0x00];

    private static readonly InvalidOperationException ResponseFormatError = new("Invalid full status response format");

    public override FullStatus Parse(byte[] data)
    {
        SessionId sessionId = StartParsing(data, out SequenceReader<byte> reader);

        if (!reader.IsNext(Constant1, true))
        {
            throw ResponseFormatError;
        }

        Dictionary<string, string> statusKeyValues = new();
        while (!reader.IsNext(0, true))
        {
            string key = ParseNullTerminatingString(ref reader);
            string value = ParseNullTerminatingString(ref reader);
            statusKeyValues.Add(key, value);
        }

        if (!reader.IsNext(Constant2, true))
        {
            throw ResponseFormatError;
        }

        List<string> players = [];
        while (!reader.IsNext(0, true))
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
