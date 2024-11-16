namespace McQuery.Net.Data.Parsers;

internal interface IResponseParser<out T>
{
    T Parse(byte[] data);
}
