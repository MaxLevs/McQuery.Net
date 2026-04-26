using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using McQuery.Net.Data;
using McQuery.Net.Internal.Abstract;
using McQuery.Net.Internal.Data;
using McQuery.Net.Internal.Factories;
using McQuery.Net.Internal.Helpers;
using McQuery.Net.Internal.Parsers;
using McQuery.Net.Internal.Providers;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;

namespace McQuery.Net;

/// <summary>
/// Implementation of <see cref="IMcQueryClient"/>.
/// </summary>
[UsedImplicitly]
public class McQueryMcQueryClient : IMcQueryClient, IAuthOnlyMcQueryClient
{
    [SuppressMessage("Usage", "VSTHRD012:Provide JoinableTaskFactory where allowed")]
    private readonly AsyncReaderWriterLock _locker = new();

    private readonly UdpClient _socket;
    private readonly IRequestFactory _requestFactory;
    private readonly ISessionStorage _sessionStorage;
    private readonly ILogger _logger;
    private readonly IResponseParser<ChallengeToken> _handshakeResponseParser;
    private readonly IResponseParser<BasicStatus> _basicStatusResponseParser;
    private readonly IResponseParser<FullStatus> _fullStatusResponseParser;

    private const int ResponseTimeoutSeconds = 5; // TODO: into the config

    internal McQueryMcQueryClient(
        UdpClient socket,
        IRequestFactory requestFactory,
        ISessionStorage sessionStorage,
        ILogger<McQueryMcQueryClient> logger
    )
    {
        _requestFactory = requestFactory;
        _sessionStorage = sessionStorage;
        _logger = logger;
        _socket = socket;
        _handshakeResponseParser = new HandshakeResponseParser(logger);
        _basicStatusResponseParser = new BasicStatusResponseParser(logger);
        _fullStatusResponseParser = new FullStatusResponseParser(logger);
    }

    /// <inheritdoc />
    public async Task<BasicStatus> GetBasicStatusAsync(IPEndPoint serverEndpoint, CancellationToken cancellationToken) =>
        await SendRequestAsync(
            serverEndpoint,
            session => _requestFactory.GetBasicStatusRequest(session),
            _basicStatusResponseParser,
            cancellationToken);

    /// <inheritdoc />
    public async Task<FullStatus> GetFullStatusAsync(IPEndPoint serverEndpoint, CancellationToken cancellationToken) =>
        await SendRequestAsync(
            serverEndpoint,
            session => _requestFactory.GetFullStatusRequest(session),
            _fullStatusResponseParser,
            cancellationToken);

    /// <inheritdoc />
    async Task<ChallengeToken> IAuthOnlyMcQueryClient.HandshakeAsync(
        IPEndPoint serverEndpoint,
        SessionId sessionId,
        CancellationToken cancellationToken
    )
    {
        var packet = _requestFactory.GetHandshakeRequest(sessionId);
        return await SendRequestAsync(
            serverEndpoint,
            packet,
            _handshakeResponseParser,
            cancellationToken);
    }

    private async Task<T> SendRequestAsync<T>(
        IPEndPoint serverEndpoint,
        Func<Session, ReadOnlyMemory<byte>> packetFactory,
        IResponseParser<T> responseParser,
        CancellationToken cancellationToken = default
    )
    {
        var session = await _sessionStorage.GetAsync(serverEndpoint, cancellationToken);
        var packet = packetFactory(session);
        return await SendRequestAsync(
            serverEndpoint,
            packet,
            responseParser,
            cancellationToken);
    }

    private async Task<T> SendRequestAsync<T>(
        IPEndPoint serverEndpoint,
        ReadOnlyMemory<byte> packet,
        IResponseParser<T> responseParser,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug(
            "Sending {PacketLength} bytes to {Endpoint} with content {Content}",
            packet.Length,
            serverEndpoint,
            BitConverter.ToString(packet.ToArray()));

        var response = await ExecuteRequestConcurrentlyAsync();

        _logger.LogDebug(
            "Received response from server {Endpoint} [{Content}]",
            serverEndpoint,
            BitConverter.ToString(response.Buffer));

        var responseData = responseParser.Parse(response.Buffer);
        _logger.LogDebug(
            "Parsed response from server {Endpoint} \n{Response}",
            serverEndpoint,
            responseData);

        return responseData;

        async Task<UdpReceiveResult> ExecuteRequestConcurrentlyAsync()
        {
            using var timeoutSource = cancellationToken.ToSourceWithTimeout(TimeSpan.FromSeconds(ResponseTimeoutSeconds));
            await using var _ = await _locker.WriteLockAsync(timeoutSource.Token);

            await _socket.SendAsync(packet, serverEndpoint, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            return await _socket.ReceiveAsync(timeoutSource.Token).ConfigureAwait(continueOnCapturedContext: false);
        }
    }

    private bool _isDisposed;

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;

        _sessionStorage.Dispose();
        _socket.Dispose();
        _locker.Dispose();
        GC.SuppressFinalize(this);
    }
}
