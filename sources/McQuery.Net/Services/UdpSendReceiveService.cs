using System.Net;
using System.Net.Sockets;
using McQuery.Net.Data;
using McQuery.Net.Data.Packages;
using McQuery.Net.Data.Packages.Responses;

namespace McQuery.Net.Services;

// todo: add resend N times before returning TimeoutResponse
// todo: add cancellation token support
public class UdpSendReceiveService
{
    public UdpSendReceiveService(int receiveAwaitInterval)
    {
        ReceiveAwaitInterval = receiveAwaitInterval;
    }

    public int ReceiveAwaitInterval { get; set; }

    public async Task<IResponse> SendReceive(Server server, Request request)
    {
        UdpClient client = server.UdpClient;

        IPEndPoint? ipEndPoint = null;
        byte[]? response = null;

        await server.UdpClientSemaphoreSlim.WaitAsync();
        await server.UdpClient.SendAsync(request.RawRequestData, request.RawRequestData.Length);
        IAsyncResult responseToken;

        try
        {
            responseToken = server.UdpClient.BeginReceive(null, null);
        }
        catch (SocketException)
        {
            server.UdpClientSemaphoreSlim.Release();

            return new TimeoutResponse(server.UUID);
        }

        responseToken.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(ReceiveAwaitInterval));
        if (responseToken.IsCompleted)
        {
            try
            {
                response = server.UdpClient.EndReceive(responseToken, ref ipEndPoint);
            }

            catch (Exception)
            {
                server.UdpClientSemaphoreSlim.Release();

                return new TimeoutResponse(server.UUID);
            }
        }
        else
        {
            server.UdpClientSemaphoreSlim.Release();

            return new TimeoutResponse(server.UUID);
        }

        if (response == null)
        {
            server.UdpClientSemaphoreSlim.Release();

            return new TimeoutResponse(server.UUID);
        }

        server.UdpClientSemaphoreSlim.Release();

        return new RawResponse(server.UUID, response);
    }
}
