using McQuery.Net.Data;
using McQuery.Net.Utils;

namespace McQuery.Net.Services;

public class SessionIdProviderService
{
    public SessionIdProviderService(Random random)
    {
        this.random = random;
        ReservedIds = new List<SessionId>();
        IdCounter = new ByteCounter();
    }


    private readonly List<SessionId> ReservedIds;

    private Random random;

    public SessionId GenerateRandomId()
    {
        byte[] sessionIdData = new byte[4];
        SessionId sessionId;

        do
        {
            random.NextBytes(sessionIdData);
            for (int i = 0; i < sessionIdData.Length; ++i) sessionIdData[i] &= 0x0F;

            sessionId = new SessionId(sessionIdData);
        } while (IsIdReserved(sessionId));

        ReserveId(sessionId);

        return sessionId;
    }


    private readonly ByteCounter IdCounter = new();

    public SessionId GetUinqueId()
    {
        byte[] sessionIdData = new byte[4];
        if (!IdCounter.GetNext(sessionIdData))
        {
            // find released sessionIds
        }

        SessionId sessionId = new(sessionIdData);
        ReserveId(sessionId);

        return sessionId;
    }

    private void ReserveId(SessionId sessionId)
    {
        ReservedIds.Add(sessionId);
    }

    public bool IsIdReserved(SessionId sessionId) => ReservedIds.IndexOf(sessionId) != -1;
}
