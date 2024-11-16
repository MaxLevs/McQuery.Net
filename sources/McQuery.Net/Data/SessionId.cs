namespace McQuery.Net.Data;

/// <summary>
/// This class represents SessionId filed into packages.
/// It provides api for create random SessionId or parse it from byte[]
/// </summary>
public class SessionId
{
    private readonly byte[] sessionId;

    public SessionId(byte[] sessionId)
    {
        this.sessionId = sessionId;
    }

    public string GetString() => BitConverter.ToString(sessionId);

    public byte[] GetBytes()
    {
        byte[] sessionIdSnapshot = new byte[4];
        Buffer.BlockCopy(sessionId, 0, sessionIdSnapshot, 0, 4);

        return sessionIdSnapshot;
    }

    public void WriteTo(List<byte> list)
    {
        list.AddRange(sessionId);
    }

    public override bool Equals(object? obj) =>
        obj is SessionId anotherSessionId && sessionId.SequenceEqual(anotherSessionId.sessionId);

    public override int GetHashCode() => BitConverter.ToInt32(sessionId, 0);
}
