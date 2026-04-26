using McQuery.Net.Data;
using McQuery.Net.Exceptions;

namespace McQuery.Net.Internal.Parsers;

internal class BasicStatusResponseParser : StatusResponseParser<BasicStatus>
{
    public override BasicStatus Parse(byte[] data)
    {
        try
        {

            var sessionId = StartParsing(data, out var reader);

            var motd = ParseNullTerminatingString(ref reader);
            var gameType = ParseNullTerminatingString(ref reader);
            var map = ParseNullTerminatingString(ref reader);
            var numPlayers = int.Parse(ParseNullTerminatingString(ref reader));
            var maxPlayers = int.Parse(ParseNullTerminatingString(ref reader));
            var hostPort = ParseShortLittleEndian(ref reader);
            var hostIp = ParseNullTerminatingString(ref reader);

            return new BasicStatus(
                motd,
                gameType,
                map,
                numPlayers,
                maxPlayers,
                hostPort,
                hostIp)
            {
                SessionId = sessionId,
            };
        }

        catch (Exception exception)
        {
            throw new McQueryException("Invalid basic status response format", exception);
        }
    }
}
