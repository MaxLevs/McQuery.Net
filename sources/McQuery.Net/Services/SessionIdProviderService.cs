using McQuery.Net.Data;
using McQuery.Net.Utils;

namespace McQuery.Net.Services;

public class SessionIdProviderService
{
    public SessionIdProviderService(Random random)
    {
        this.random = random;
        reservedIds = [];
        idCounter = new ByteCounter();
    }


    private readonly List<SessionId> reservedIds;

    private readonly Random random;

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


    private readonly ByteCounter idCounter;

    public SessionId GetUniqueId()
    {
        byte[] sessionIdData = new byte[4];
        if (!idCounter.GetNext(sessionIdData))
        {
            // find released sessionIds
        }

        SessionId sessionId = new(sessionIdData);
        ReserveId(sessionId);

        return sessionId;
    }

    private void ReserveId(SessionId sessionId)
    {
        reservedIds.Add(sessionId);
    }

    public bool IsIdReserved(SessionId sessionId) => reservedIds.IndexOf(sessionId) != -1;
}
