using McQuery.Net.Data;
using McQuery.Net.Exceptions;
using Microsoft.Extensions.Logging;

namespace McQuery.Net.Internal.Parsers;

internal class FullStatusResponseParser : StatusResponseParser<FullStatus>
{
    private static readonly byte[] constant1 = [0x73, 0x70, 0x6C, 0x69, 0x74, 0x6E, 0x75, 0x6D, 0x00, 0x80, 0x00];
    private static readonly byte[] constant2 = [0x01, 0x70, 0x6C, 0x61, 0x79, 0x65, 0x72, 0x5F, 0x00, 0x00];

    public FullStatusResponseParser(ILogger logger)
        : base(logger)
    {
    }

    public override FullStatus Parse(byte[] data)
    {
        var sessionId = StartParsing(data, out var reader);

        if (!reader.IsNext(constant1, advancePast: true))
        {
            Logger.LogError("Invalid full status response format: [{Data}]", BitConverter.ToString(data));
            throw new McQueryResponseParsingException("Invalid full status response format");
        }

        Dictionary<string, string> statusKeyValues = new();
        while (!reader.IsNext(next: 0, advancePast: true))
        {
            var key = ParseNullTerminatingString(ref reader);
            var value = ParseNullTerminatingString(ref reader);
            statusKeyValues.Add(key, value);
        }

        if (!reader.IsNext(constant2, advancePast: true))
        {
            Logger.LogError("Invalid full status response format: [{Data}]", BitConverter.ToString(data));
            throw new McQueryResponseParsingException("Invalid full status response format");
        }

        List<string> playerList = [];
        while (!reader.IsNext(next: 0, advancePast: true))
        {
            playerList.Add(ParseNullTerminatingString(ref reader));
        }

        try
        {
            var motd = statusKeyValues["hostname"];
            var gametype = statusKeyValues["gametype"];
            var gameId = statusKeyValues["game_id"];
            var version = statusKeyValues["version"];
            var plugins = statusKeyValues["plugins"];
            var map = statusKeyValues["map"];
            var numPlayers = int.Parse(statusKeyValues["numplayers"]);
            var maxPlayers = int.Parse(statusKeyValues["maxplayers"]);
            var hostPort = int.Parse(statusKeyValues["hostport"]);
            var hostIp = statusKeyValues["hostip"];

            return new FullStatus(
                motd,
                gametype,
                gameId,
                version,
                plugins,
                map,
                numPlayers,
                maxPlayers,
                playerList.ToArray(),
                hostPort,
                hostIp)
            {
                SessionId = sessionId,
            };
        }
        catch (McQueryException)
        {
            throw;
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "Invalid full status response format: [{Data}]", BitConverter.ToString(data));
            throw new McQueryResponseParsingException("Invalid full status response format", exception);
        }
    }
}
