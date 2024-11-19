namespace McQuery.Net;

[PublicAPI]
public interface IMcQueryClientFactory
{
    IMcQueryClient Get();
}
