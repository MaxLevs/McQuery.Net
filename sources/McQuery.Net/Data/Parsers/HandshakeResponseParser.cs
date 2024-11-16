using System.Buffers;

namespace McQuery.Net.Data.Parsers;

internal class HandshakeResponseParser : ResponseParserBase, IResponseParser<ChallengeToken>
{
    public override byte ResponseType => 0x09;

    public ChallengeToken Parse(byte[] data)
    {
        StartParsing(data, out SequenceReader<byte> reader);

        string challengeTokenString = ParseNullTerminatingString(ref reader);
        byte[] challengeTokenBytes = BitConverter.GetBytes(int.Parse(challengeTokenString));

        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(challengeTokenBytes);
        }

        return new ChallengeToken(challengeTokenBytes);
    }
}
