﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MCQueryLib.Data;
using MCQueryLib.Data.Packages;
using MCQueryLib.Data.Packages.Responses;

namespace MCQueryLib.Services
{

    // todo: add resend N times before returning TimeoutResponce
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
            var client = server.UdpClient;

            IPEndPoint ipEndPoint = null;
            byte[] response = null;

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
                return new TimeoutResponse();
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
                    return new TimeoutResponse();
                }
            }
            else
            {
                server.UdpClientSemaphoreSlim.Release();
                return new TimeoutResponse();
            }

            if (response == null)
            {
                server.UdpClientSemaphoreSlim.Release();
                return new TimeoutResponse();
            }

            server.UdpClientSemaphoreSlim.Release();
            return new RawResponse(response);
        }
    }
}