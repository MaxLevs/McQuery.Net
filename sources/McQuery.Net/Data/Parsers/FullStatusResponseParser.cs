using System.Buffers;
using McQuery.Net.Data.Responses;

namespace McQuery.Net.Data.Parsers;

internal class FullStatusResponseParser : StatusResponseParser<FullStatus>
{
    private static readonly byte[] Constant1 = [0x73, 0x70, 0x6C, 0x69, 0x74, 0x6E, 0x75, 0x6D, 0x00, 0x80, 0x00];

    private static readonly byte[] Constant2 = [0x01, 0x70, 0x6C, 0x61, 0x79, 0x65, 0x72, 0x5F, 0x00, 0x00];

    public override FullStatus Parse(byte[] data)
    {
        SessionId sessionId = StartParsing(data, out SequenceReader<byte> reader);

        throw new NotImplementedException();
    }
}
