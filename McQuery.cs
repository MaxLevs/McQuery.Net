using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MCQueryLib.Packages;
using MCQueryLib.State;
using UdpExtension;

namespace MCQueryLib
{
    /// <summary>
    /// This class provides api for getting status data from Minecraft server via Minecraft Server Query protocol.
    /// It doesn't manage networking. You should create your own manager for your own needs
    /// </summary>
    public class McQuery
    {
        public IPAddress Host { get; }
        public int Port { get; }
        
        private UdpClient _udpClient = null;
        public int ResponseWaitIntervalSecond = 10;
        
        private object _challengeTokenLock = new();
        private byte[] _challengeToken = null;

        public bool IsOnline { get; set; } = true;

        private void SetChallengeToken(byte[] challengeToken)
        {
            lock (_challengeTokenLock)
            {
                _challengeToken ??= new byte[4];
                Buffer.BlockCopy(challengeToken, 0, _challengeToken, 0, 4);
            }
        }

        private byte[] GetChallengeToken()
        {
            lock (_challengeTokenLock)
            {
                if (_challengeToken == null) 
                    return null;
            }
                
            var challengeToken = new byte[4];
            
            lock (_challengeTokenLock)
            {
                Buffer.BlockCopy(_challengeToken, 0, challengeToken, 0, 4);
            }

            return challengeToken;
        }
        
        public McQuery(IPAddress host, int queryPort)
        {
            Host = host;
            Port = queryPort;
        }

        public void InitSocket()
        {
            _udpClient?.Dispose();
            _udpClient = null;
            _udpClient = new UdpClient(Host.ToString(), Port);
        }

        public async Task<byte[]> GetHandshake()
        {
            if (_udpClient == null)
                throw new McQuerySocketIsNotInitialised(this);
            
            Request handshakeRequest = Request.GetHandshakeRequest();
            var response = await SendResponseService.SendReceive(_udpClient, handshakeRequest.Data, ResponseWaitIntervalSecond);

            var challengeToken = Response.ParseHandshake(response);
            SetChallengeToken(challengeToken);
            
            return challengeToken;
        }

        public async Task<ServerBasicState> GetBasicStatus()
        {
            if (_udpClient == null)
                throw new McQuerySocketIsNotInitialised(this);

            if (!IsOnline)
                throw new McQueryServerIsOffline(this);

            var challengeToken = GetChallengeToken();
            
            if (challengeToken == null)
                throw new McQueryServerIsOffline(this);
            
            Request basicStatusRequest = Request.GetBasicStatusRequest(challengeToken);
            var response = await SendResponseService.SendReceive(_udpClient, basicStatusRequest.Data, ResponseWaitIntervalSecond);
            
            var basicStatus = Response.ParseBasicState(response);
            return basicStatus;
        }
        
        public async Task<ServerFullState> GetFullStatus()
        {
            if (_udpClient == null)
                throw new McQuerySocketIsNotInitialised(this);

            if (!IsOnline)
                throw new McQueryServerIsOffline(this);

            var challengeToken = GetChallengeToken();
            
            Request fullStatusRequest = Request.GetFullStatusRequest(challengeToken);
            var response = await SendResponseService.SendReceive(_udpClient, fullStatusRequest.Data, ResponseWaitIntervalSecond);

            var fullStatus = Response.ParseFullState(response);
            return fullStatus;
        }
    }

    public class McQueryServerIsOffline : Exception
    {
        public McQuery Query { get; }
        public McQueryServerIsOffline(McQuery mcQuery)
        {
            Query = mcQuery;
        }
    }

    public class McQuerySocketIsNotInitialised : Exception
    {
        public McQuery Query { get; }
        
        public McQuerySocketIsNotInitialised(McQuery query)
        {
            Query = query;
        }

        protected McQuerySocketIsNotInitialised(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public McQuerySocketIsNotInitialised(string? message) : base(message)
        {
        }

        public McQuerySocketIsNotInitialised(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}