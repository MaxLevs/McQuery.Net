using McQuery.Net.Data.Responses;

namespace McQuery.Net.Data.Parsers;

internal abstract class StatusResponseParser<T> : ResponseParserBase, IResponseParser<T> where T : StatusBase
{
    public override byte ResponseType => 0x00;

    public abstract T Parse(byte[] data);
}
