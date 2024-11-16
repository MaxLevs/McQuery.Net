using System.Net;
using System.Net.Sockets;

namespace McQuery.Net.Data;

// todo: add cancellation token support
public class Server : IDisposable
{
    public Server(SessionId sessionId, IPAddress host, int port)
    {
        UUID = Guid.NewGuid();
        SessionId = sessionId;
        Host = host;
        Port = port;
        ChallengeToken = new ChallengeToken();
        UdpClient = new UdpClient(Host.ToString(), Port);
        UdpClientSemaphoreSlim = new SemaphoreSlim(0, 1);
        UdpClientSemaphoreSlim.Release();
    }

    public Guid UUID { get; }

    public SessionId SessionId { get; }

    public IPAddress Host { get; }

    public int Port { get; }

    public ChallengeToken ChallengeToken { get; }

    public UdpClient UdpClient { get; private set; }

    public SemaphoreSlim UdpClientSemaphoreSlim { get; }

    public async void InvalidateSocket()
    {
        await UdpClientSemaphoreSlim.WaitAsync();
        UdpClient.Dispose();
        UdpClient = new UdpClient(Host.ToString(), Port);
        UdpClientSemaphoreSlim.Release();
    }

    private bool disposed = false;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                UdpClient.Dispose();
                UdpClientSemaphoreSlim.Dispose();
            }

            disposed = true;
        }
    }

    ~Server()
    {
        Dispose(true);
    }

    public override bool Equals(object? obj) => obj is Server server &&
                                                EqualityComparer<Guid>.Default.Equals(UUID, server.UUID);

    public override int GetHashCode() => UUID.GetHashCode();
}
