namespace McQuery.Net.Internal.Parsers;

internal interface IResponseParser<out T>
{
    T Parse(byte[] data);
}
