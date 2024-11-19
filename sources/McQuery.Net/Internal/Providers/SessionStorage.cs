using System.Diagnostics.CodeAnalysis;
using System.Net;
using McQuery.Net.Internal.Abstract;
using McQuery.Net.Internal.Data;
using Microsoft.VisualStudio.Threading;

namespace McQuery.Net.Internal.Providers;

/// <summary>
/// Implementation of <see cref="ISessionStorage"/>.
/// </summary>
/// <param name="sessionIdProvider"><see cref="IServiceProvider"/>.</param>
internal class SessionStorage(ISessionIdProvider sessionIdProvider) : ISessionStorage
{
    [SuppressMessage("Usage", "VSTHRD012:Provide JoinableTaskFactory where allowed")]
    private readonly AsyncReaderWriterLock _locker = new();

    private IAuthOnlyClient? _authClient;
    private readonly Dictionary<IPEndPoint, Session> _sessionsByEndpoints = new();

    /// <inheritdoc />
    public async Task<Session> GetAsync(IPEndPoint serverEndpoint, CancellationToken cancellationToken = default)
    {
        await using var releaser = await _locker.UpgradeableReadLockAsync(cancellationToken);

        var sessionExists = _sessionsByEndpoints.TryGetValue(serverEndpoint, out var session);

        if (sessionExists && !session!.IsExpired)
        {
            return session;
        }

        if (sessionExists && session!.IsExpired)
        {
            if (!_sessionsByEndpoints.Remove(serverEndpoint, out session))
            {
                throw new Exception($"Cannot remove expired session {session} for some reason.");
            }
        }

        return await AcquireSessionAsync(serverEndpoint, cancellationToken);
    }

    internal void Init(IAuthOnlyClient client)
    {
        if (_authClient != null) throw new InvalidOperationException("SessionStorage already initialized.");

        _authClient = client;
    }

    private async Task<Session> AcquireSessionAsync(IPEndPoint serverEndpoint, CancellationToken cancellationToken)
    {
        await using var releaser = await _locker.WriteLockAsync(cancellationToken);

        var sessionExists = _sessionsByEndpoints.TryGetValue(serverEndpoint, out var currentSession);
        if (sessionExists && !currentSession!.IsExpired)
        {
            return currentSession;
        }

        if (_authClient == null)
        {
            throw new InvalidOperationException("Storage must be initialized before calling this method.");
        }

        var sessionId = sessionIdProvider.Get();
        var challengeToken = await _authClient!.HandshakeAsync(serverEndpoint, sessionId, cancellationToken);
        Session session = new(sessionId, challengeToken);

        return _sessionsByEndpoints[serverEndpoint] = session;
    }

    private bool _isDisposed;

    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        _authClient?.Dispose();
        GC.SuppressFinalize(this);
    }
}
