using System.Net;
using System.Net.Sockets;
using McQuery.Net.Abstract;
using McQuery.Net.Data;
using McQuery.Net.Data.Factories;
using McQuery.Net.Data.Parsers;
using McQuery.Net.Data.Providers;
using McQuery.Net.Data.Responses;

namespace McQuery.Net;

[PublicAPI]
public class McQueryClient : IMcQueryClient, IAuthOnlyClient
{
    private readonly UdpClient socket;
    private readonly IRequestFactory requestFactory;
    private readonly ISessionStorage sessionStorage;
    private int responseTimeoutSeconds = 5; // Temp value

    private static readonly IResponseParser<ChallengeToken> HandshakeResponseParser = new HandshakeResponseParser();
    private static readonly IResponseParser<BasicStatus> BasicStatusResponseParser = new BasicStatusResponseParser();
    private static readonly IResponseParser<FullStatus> FullStatusResponseParser = new ThrowResponseParser<FullStatus>();

    internal McQueryClient(UdpClient socket, IRequestFactory requestFactory, ISessionStorage sessionStorage)
    {
        this.requestFactory = requestFactory;
        this.sessionStorage = sessionStorage;
        this.socket = socket;
    }

    public async Task<BasicStatus> GetBasicStatusAsync(IPEndPoint serverEndpoint, CancellationToken cancellationToken = default) =>
        await SendRequestAsync(
            serverEndpoint,
            session => requestFactory.GetBasicStatusRequest(session),
            BasicStatusResponseParser,
            cancellationToken);

    public async Task<FullStatus> GetFullStatusAsync(IPEndPoint serverEndpoint, CancellationToken cancellationToken = default) =>
        await SendRequestAsync(
            serverEndpoint,
            session => requestFactory.GetFullStatusRequest(session),
            FullStatusResponseParser,
            cancellationToken);

    public BasicStatus GetBasicStatus(IPEndPoint serverEndpoint, CancellationToken cancellationToken = default) =>
        GetBasicStatusAsync(serverEndpoint, cancellationToken).GetAwaiter().GetResult();

    public FullStatus GetFullStatus(IPEndPoint serverEndpoint, CancellationToken cancellationToken = default) =>
        GetFullStatusAsync(serverEndpoint, cancellationToken).GetAwaiter().GetResult();

    async Task<ChallengeToken> IAuthOnlyClient.HandshakeAsync(
        IPEndPoint serverEndpoint,
        SessionId sessionId,
        CancellationToken cancellationToken)
    {
        byte[] packet = requestFactory.GetHandshakeRequest(sessionId);

        return await SendRequestAsync(
            serverEndpoint,
            packet,
            HandshakeResponseParser,
            cancellationToken);
    }

    private async Task<T> SendRequestAsync<T>(
        IPEndPoint serverEndpoint,
        Func<Session, ReadOnlyMemory<byte>> packetFactory,
        IResponseParser<T> responseParser,
        CancellationToken cancellationToken = default)
    {
        Session session = await sessionStorage.GetAsync(serverEndpoint, cancellationToken);
        ReadOnlyMemory<byte> packet = packetFactory(session);
        return await SendRequestAsync(serverEndpoint, packet, responseParser, cancellationToken);
    }

    private async Task<T> SendRequestAsync<T>(
        IPEndPoint serverEndpoint,
        ReadOnlyMemory<byte> packet,
        IResponseParser<T> responseParser,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Sending {packet.Length} bytes to {serverEndpoint} with content {BitConverter.ToString(packet.ToArray())}");

        await socket.SendAsync(packet, serverEndpoint, cancellationToken).ConfigureAwait(false);

        using CancellationTokenSource timeoutSource = new(TimeSpan.FromSeconds(responseTimeoutSeconds));
        using CancellationTokenSource linkedSource =
            CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutSource.Token);

        CancellationToken tokenWithTimeout = linkedSource.Token;
        // TODO: common response pool
        UdpReceiveResult response = await socket.ReceiveAsync(tokenWithTimeout).ConfigureAwait(false);

        Console.WriteLine($"Received response from server: {BitConverter.ToString(response.Buffer)}");
        T responseData = responseParser.Parse(response.Buffer);
        Console.WriteLine(responseData);

        return responseData;
    }


    private bool isDisposed;
    public void Dispose()
    {
        if(isDisposed) return;

        socket.Dispose();
        sessionStorage.Dispose();
        GC.SuppressFinalize(this);
        isDisposed = true;
    }
}

// TODO: remove
internal class ThrowResponseParser<T> : IResponseParser<T>
{
    public T Parse(byte[] data) => throw new NotImplementedException();
}
