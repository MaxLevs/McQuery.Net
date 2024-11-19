using System.Net.Sockets;
using McQuery.Net.Internal.Factories;
using McQuery.Net.Internal.Providers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace McQuery.Net;

[UsedImplicitly]
public class McQueryClientFactory : IMcQueryClientFactory
{
    private readonly ILoggerFactory? _loggerFactory;
    private readonly Lazy<ISessionIdProvider> _sessionIdProvider;
    private readonly Lazy<IMcQueryClient> _client;

    public McQueryClientFactory(ILoggerFactory? loggerFactory = null)
    {
        _loggerFactory = loggerFactory;
        _sessionIdProvider = new Lazy<ISessionIdProvider>(() => new SessionIdProvider(), isThreadSafe: true);
       _client = new Lazy<IMcQueryClient>(AcquireClient, isThreadSafe: true);
    }

    public IMcQueryClient Get() => _client.Value;


    private IMcQueryClient AcquireClient()
    {
        SessionStorage sessionStorage = new(_sessionIdProvider.Value);

        McQueryClient client = new(
            new UdpClient(),
            new RequestFactory(),
            sessionStorage,
            _loggerFactory?.CreateLogger<McQueryClient>() ?? new NullLogger<McQueryClient>());
        sessionStorage.Init(client);

        return client;
    }
}
