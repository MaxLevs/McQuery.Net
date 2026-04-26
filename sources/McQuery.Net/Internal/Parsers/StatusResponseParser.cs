using McQuery.Net.Data;
using Microsoft.Extensions.Logging;

namespace McQuery.Net.Internal.Parsers;

internal abstract class StatusResponseParser<T> : ResponseParserBase, IResponseParser<T>
    where T : StatusBase
{
    protected StatusResponseParser(ILogger logger)
        : base(logger)
    {
    }

    protected override byte ResponseType => 0x00;

    public abstract T Parse(byte[] data);
}
