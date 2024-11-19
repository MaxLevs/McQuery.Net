using McQuery.Net.Data;

namespace McQuery.Net.Internal.Parsers;

internal abstract class StatusResponseParser<T> : ResponseParserBase, IResponseParser<T>
    where T : StatusBase
{
    protected override byte ResponseType => 0x00;

    public abstract T Parse(byte[] data);
}
