using System.Net.Sockets;
using McQuery.Net.Internal.Factories;
using McQuery.Net.Internal.Providers;

namespace McQuery.Net;

[UsedImplicitly]
public class McQueryClientFactory : IMcQueryClientFactory
{
    private readonly Lazy<ISessionIdProvider> _sessionIdProvider = new(() => new SessionIdProvider(), isThreadSafe: true);

    public IMcQueryClient Get()
    {
        SessionStorage sessionStorage = new(_sessionIdProvider.Value);

        McQueryClient client = new(
            new UdpClient(),
            new RequestFactory(),
            sessionStorage);
        sessionStorage.Init(client);

        return client;
    }
}
