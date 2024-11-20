using McQuery.Net.Internal.Data;

namespace McQuery.Net.Internal.Parsers;

internal class HandshakeResponseParser : ResponseParserBase, IResponseParser<ChallengeToken>
{
    protected override byte ResponseType => 0x09;

    public ChallengeToken Parse(byte[] data)
    {
        StartParsing(data, out var reader);

        var challengeTokenString = ParseNullTerminatingString(ref reader);
        var challengeTokenBytes = BitConverter.GetBytes(int.Parse(challengeTokenString));

        if (BitConverter.IsLittleEndian) Array.Reverse(challengeTokenBytes);

        return new ChallengeToken(challengeTokenBytes);
    }
}
