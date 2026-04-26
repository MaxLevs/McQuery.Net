using McQuery.Net.Data;
using McQuery.Net.Exceptions;
using Microsoft.Extensions.Logging;

namespace McQuery.Net.Internal.Parsers;

internal class BasicStatusResponseParser : StatusResponseParser<BasicStatus>
{
    public BasicStatusResponseParser(ILogger logger) : base(logger)
    {
    }

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
        catch (McQueryException)
        {
            throw;
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "Invalid handshake status response format: [{Data}]", BitConverter.ToString(data));
            throw new McQueryResponseParsingException("Invalid basic status response format", exception);
        }
    }
}
