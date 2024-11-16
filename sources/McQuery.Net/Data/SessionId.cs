namespace McQuery.Net.Data;

/// <summary>
/// This class represents SessionId filed into packages.
/// It provides api for create random SessionId or parse it from byte[]
/// </summary>
public class SessionId
{
    private readonly byte[] _sessionId;

    public SessionId(byte[] sessionId)
    {
        _sessionId = sessionId;
    }

    public string GetString() => BitConverter.ToString(_sessionId);

    public byte[] GetBytes()
    {
        byte[]? sessionIdSnapshot = new byte[4];
        Buffer.BlockCopy(_sessionId, 0, sessionIdSnapshot, 0, 4);

        return sessionIdSnapshot;
    }

    public void WriteTo(List<byte> list)
    {
        list.AddRange(_sessionId);
    }

    public override bool Equals(object? obj) =>
        obj is SessionId anotherSessionId && _sessionId.SequenceEqual(anotherSessionId._sessionId);

    public override int GetHashCode() => BitConverter.ToInt32(_sessionId, 0);
}
