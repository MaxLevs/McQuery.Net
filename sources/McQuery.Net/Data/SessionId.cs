namespace McQuery.Net.Data;

/// <summary>
/// Represents Session Identifier.
/// </summary>
[PublicAPI]
public class SessionId
{
    private byte[] Data { get; }

    /// <summary>
    /// .ctor.
    /// </summary>
    /// <param name="data">Bytes that represents session identifier.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Number of bytes is incorrect.
    /// </exception>
    public SessionId(byte[] data)
    {
        if (data.Length != 4)
            throw new ArgumentOutOfRangeException(nameof(data), data, "Session identifier must have 4 bytes");

        Data = data;
    }

    public static implicit operator string(SessionId sessionId)
    {
        return BitConverter.ToString(sessionId.Data);
    }

    public static implicit operator byte[](SessionId sessionId)
    {
        return [..sessionId.Data];
    }

    public override bool Equals(object? obj)
    {
        return obj is SessionId anotherSessionId
               && Data.SequenceEqual(anotherSessionId.Data);
    }

    public override int GetHashCode()
    {
        return BitConverter.ToInt32(Data, 0);
    }
}
