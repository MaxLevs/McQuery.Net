using System.Collections.Concurrent;
using System.Net;
using McQuery.Net.Abstract;

namespace McQuery.Net.Data.Providers;

/// <summary>
/// Implementation of <see cref="ISessionStorage"/>.
/// </summary>
/// <param name="sessionIdProvider"><see cref="IServiceProvider"/>.</param>
internal class SessionStorage(ISessionIdProvider sessionIdProvider) : ISessionStorage
{
    private IAuthOnlyClient? authClient;
    private readonly ConcurrentDictionary<IPEndPoint, Session> sessionsByEndpoints = new();

    /// <inheritdoc />
    public async Task<Session> GetAsync(IPEndPoint serverEndpoint, CancellationToken cancellationToken = default)
    {
        bool sessionExists = sessionsByEndpoints.TryGetValue(serverEndpoint, out Session? session);

        if (sessionExists && !session!.IsExpired)
        {
            return session;
        }

        if (sessionExists && session!.IsExpired)
        {
            if (!sessionsByEndpoints.TryRemove(serverEndpoint, out _))
            {
                throw new Exception($"Cannot remove expired session {session} for some reason.");
            }
        }

        return await AcquireSession(serverEndpoint, cancellationToken);
    }

    internal void Init(IAuthOnlyClient client)
    {
        if (authClient != null)
        {
            throw new InvalidOperationException("SessionStorage already initialized.");
        }

        authClient = client;
    }

    private async Task<Session> AcquireSession(IPEndPoint serverEndpoint, CancellationToken cancellationToken)
    {
        if (authClient == null)
        {
            throw new InvalidOperationException("Storage must be initialized before calling this method.");
        }

        SessionId sessionId = sessionIdProvider.Get();
        ChallengeToken challengeToken = await authClient!.HandshakeAsync(serverEndpoint, sessionId, cancellationToken);
        Session session = new(sessionId, challengeToken);

        if (!sessionsByEndpoints.TryAdd(serverEndpoint, session))
        {
            throw new Exception("Cannot add session for endpoint " + serverEndpoint + " for some reason.");
        }

        return session;
    }

    private bool isDisposed;
    public void Dispose()
    {
        if(isDisposed) return;

        authClient?.Dispose();
        GC.SuppressFinalize(this);
        isDisposed = true;
    }
}
