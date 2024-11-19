using System.Net;
using System.Net.Sockets;
using McQuery.Net.Data;
using McQuery.Net.Internal.Abstract;
using McQuery.Net.Internal.Data;
using McQuery.Net.Internal.Factories;
using McQuery.Net.Internal.Parsers;
using McQuery.Net.Internal.Providers;

namespace McQuery.Net;

/// <summary>
/// Implementation of <see cref="IMcQueryClient"/>.
/// </summary>
[UsedImplicitly]
public class McQueryClient : IMcQueryClient, IAuthOnlyClient
{
    private readonly UdpClient _socket;
    private readonly IRequestFactory _requestFactory;
    private readonly ISessionStorage _sessionStorage;
    private int _responseTimeoutSeconds = 5; // TODO

    private static readonly IResponseParser<ChallengeToken> handshakeResponseParser = new HandshakeResponseParser();
    private static readonly IResponseParser<BasicStatus> basicStatusResponseParser = new BasicStatusResponseParser();
    private static readonly IResponseParser<FullStatus> fullStatusResponseParser = new FullStatusResponseParser();

    internal McQueryClient(UdpClient socket, IRequestFactory requestFactory, ISessionStorage sessionStorage)
    {
        _requestFactory = requestFactory;
        _sessionStorage = sessionStorage;
        _socket = socket;
    }

    /// <inheritdoc />
    public async Task<BasicStatus> GetBasicStatusAsync(IPEndPoint serverEndpoint, CancellationToken cancellationToken)
    {
        return await SendRequestAsync(
            serverEndpoint,
            session => _requestFactory.GetBasicStatusRequest(session),
            basicStatusResponseParser,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<FullStatus> GetFullStatusAsync(IPEndPoint serverEndpoint, CancellationToken cancellationToken)
    {
        return await SendRequestAsync(
            serverEndpoint,
            session => _requestFactory.GetFullStatusRequest(session),
            fullStatusResponseParser,
            cancellationToken);
    }

    /// <inheritdoc />
    public BasicStatus GetBasicStatus(IPEndPoint serverEndpoint, CancellationToken cancellationToken) =>
        GetBasicStatusAsync(serverEndpoint, cancellationToken).GetAwaiter().GetResult();

    /// <inheritdoc />
    public FullStatus GetFullStatus(IPEndPoint serverEndpoint, CancellationToken cancellationToken) =>
        GetFullStatusAsync(serverEndpoint, cancellationToken).GetAwaiter().GetResult();

    /// <inheritdoc />
    async Task<ChallengeToken> IAuthOnlyClient.HandshakeAsync(
        IPEndPoint serverEndpoint,
        SessionId sessionId,
        CancellationToken cancellationToken
    )
    {
        var packet = _requestFactory.GetHandshakeRequest(sessionId);

        return await SendRequestAsync(
            serverEndpoint,
            packet,
            handshakeResponseParser,
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
        Console.WriteLine(
            $"Sending {packet.Length} bytes to {serverEndpoint} with content {BitConverter.ToString(packet.ToArray())}");

        await _socket.SendAsync(packet, serverEndpoint, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);

        using CancellationTokenSource timeoutSource = new(TimeSpan.FromSeconds(_responseTimeoutSeconds));
        using var linkedSource =
            CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutSource.Token);

        var tokenWithTimeout = linkedSource.Token;
        // TODO: common response pool
        var response = await _socket.ReceiveAsync(tokenWithTimeout).ConfigureAwait(continueOnCapturedContext: false);

        Console.WriteLine($"Received response from server: {BitConverter.ToString(response.Buffer)}");
        var responseData = responseParser.Parse(response.Buffer);
        Console.WriteLine(responseData);

        return responseData;
    }

    private bool _isDisposed;

    public void Dispose()
    {
        if (_isDisposed) return;

        _socket.Dispose();
        _sessionStorage.Dispose();
        GC.SuppressFinalize(this);
        _isDisposed = true;
    }
}
