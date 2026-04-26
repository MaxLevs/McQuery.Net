using System.Diagnostics.CodeAnalysis;
using System.Net;
using McQuery.Net.Exceptions;
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

    private IAuthOnlyMcQueryClient? _authClient;
    private readonly Dictionary<IPEndPoint, Session> _sessionsByEndpoints = new();

    /// <inheritdoc />
    public async Task<Session> GetAsync(IPEndPoint serverEndpoint, CancellationToken cancellationToken = default)
    {
        await using var releaser = await _locker.UpgradeableReadLockAsync(cancellationToken);

        if (_sessionsByEndpoints.TryGetValue(serverEndpoint, out var existingSession) && !existingSession.IsExpired)
        {
            return existingSession;
        }

        return await AcquireSessionAsync(serverEndpoint, cancellationToken);
    }

    internal void Init(IAuthOnlyMcQueryClient mcQueryClient)
    {
        ValidateUninitialized();

        _authClient = mcQueryClient;
    }

    private async Task<Session> AcquireSessionAsync(IPEndPoint serverEndpoint, CancellationToken cancellationToken)
    {
        ValidateInitialized();

        await using var releaser = await _locker.WriteLockAsync(cancellationToken);

        if (_sessionsByEndpoints.TryGetValue(serverEndpoint, out var existingSession))
        {
            if (!existingSession.IsExpired)
            {
                return existingSession;
            }

            if (!_sessionsByEndpoints.Remove(serverEndpoint, out existingSession))
            {
                throw new McQueryException($"Cannot remove expired session {existingSession} for some reason.");
            }
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

    private void ValidateUninitialized()
    {
        if (_authClient is not null)
        {
            throw new McQueryException("SessionStorage is already initialized.");
        }
    }

    private void ValidateInitialized()
    {
        if (_authClient is null)
        {
            throw new McQueryException("Storage must be initialized before calling this method.");
        }
    }
}
