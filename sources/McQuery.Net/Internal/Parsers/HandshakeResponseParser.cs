using McQuery.Net.Exceptions;
using McQuery.Net.Internal.Data;
using Microsoft.Extensions.Logging;

namespace McQuery.Net.Internal.Parsers;

internal class HandshakeResponseParser : ResponseParserBase, IResponseParser<ChallengeToken>
{
    public HandshakeResponseParser(ILogger logger) : base(logger)
    {
    }

    protected override byte ResponseType => 0x09;

    public ChallengeToken Parse(byte[] data)
    {
        try
        {
            StartParsing(data, out var reader);

            var challengeTokenString = ParseNullTerminatingString(ref reader);
            var challengeTokenBytes = BitConverter.GetBytes(int.Parse(challengeTokenString));

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(challengeTokenBytes);
            }

            return new ChallengeToken(challengeTokenBytes);
        }
        catch (McQueryException)
        {
            throw;
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "Invalid handshake status response format: [{Data}]", BitConverter.ToString(data));
            throw new McQueryResponseParsingException("Invalid handshake format", exception);
        }
    }
}
