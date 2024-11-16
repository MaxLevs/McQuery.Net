using McQuery.Net.Data;

namespace McQuery.Net.Factories;

internal interface IRequestFactory
{
    internal byte[] GetHandshakeRequest(SessionId sessionId);
    internal byte[] GetBasicStatusRequest(Session session);
    internal byte[] GetFullStatusRequest(Session session);
}
