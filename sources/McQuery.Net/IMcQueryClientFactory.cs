using System.Net.Sockets;
using McQuery.Net.Data.Factories;
using McQuery.Net.Data.Providers;

namespace McQuery.Net;

public interface IMcQueryClientFactory
{
    IMcQueryClient Get();
}

public class McQueryClientFactory : IMcQueryClientFactory
{
    private readonly Lazy<ISessionIdProvider> sessionIdProvider = new(() => new SessionIdProvider(), isThreadSafe: true);

    public IMcQueryClient Get()
    {
        SessionStorage sessionStorage = new(sessionIdProvider.Value);

        McQueryClient client = new(
            new UdpClient(),
            new RequestFactory(),
            sessionStorage);
        sessionStorage.Init(client);

        return client;
    }
}
