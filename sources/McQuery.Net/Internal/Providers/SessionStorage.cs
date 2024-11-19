using System.Collections.Concurrent;
using System.Net;
using McQuery.Net.Internal.Abstract;
using McQuery.Net.Internal.Data;

namespace McQuery.Net.Internal.Providers;

/// <summary>
/// Implementation of <see cref="ISessionStorage"/>.
/// </summary>
/// <param name="sessionIdProvider"><see cref="IServiceProvider"/>.</param>
internal class SessionStorage(ISessionIdProvider sessionIdProvider) : ISessionStorage
{
    private IAuthOnlyClient? _authClient;
    private readonly ConcurrentDictionary<IPEndPoint, Session> _sessionsByEndpoints = new();

    /// <inheritdoc />
    public async Task<Session> GetAsync(IPEndPoint serverEndpoint, CancellationToken cancellationToken = default)
    {
        var sessionExists = _sessionsByEndpoints.TryGetValue(serverEndpoint, out var session);

        if (sessionExists && !session!.IsExpired)
        {
            return session;
        }

        if (sessionExists && session!.IsExpired)
        {
            if (!_sessionsByEndpoints.TryRemove(serverEndpoint, out _))
            {
                throw new Exception($"Cannot remove expired session {session} for some reason.");
            }
        }

        return await AcquireSession(serverEndpoint, cancellationToken);
    }

    internal void Init(IAuthOnlyClient client)
    {
        if (_authClient != null) throw new InvalidOperationException("SessionStorage already initialized.");

        _authClient = client;
    }

    private async Task<Session> AcquireSession(IPEndPoint serverEndpoint, CancellationToken cancellationToken)
    {
        if (_authClient == null)
        {
            throw new InvalidOperationException("Storage must be initialized before calling this method.");
        }

        var sessionId = sessionIdProvider.Get();
        var challengeToken = await _authClient!.HandshakeAsync(serverEndpoint, sessionId, cancellationToken);
        Session session = new(sessionId, challengeToken);

        if (!_sessionsByEndpoints.TryAdd(serverEndpoint, session))
        {
            throw new Exception("Cannot add session for endpoint " + serverEndpoint + " for some reason.");
        }

        return session;
    }

    private bool _isDisposed;

    public void Dispose()
    {
        if (_isDisposed) return;

        _authClient?.Dispose();
        GC.SuppressFinalize(this);
        _isDisposed = true;
    }
}
