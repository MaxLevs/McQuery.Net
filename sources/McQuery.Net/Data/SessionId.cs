namespace McQuery.Net.Data;

/// <summary>
/// Represents Session Identifier.
/// </summary>
/// <remarks>
/// Minecraft server does not validate this value but store along with <see cref="ChallengeToken"/> as long as handshake session
/// for current issuer is alive.
/// Can be rewritten by new value if current client send another one handshake request.
/// Server sends stored <see cref="SessionId"/> in every response (even if status request contains different <see cref="SessionId"/>
/// compared to handshake request, response contains actual <see cref="SessionId"/> from the last handshake request).
/// </remarks>
internal class SessionId
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

    public static implicit operator string(SessionId sessionId) =>
        BitConverter.ToString(sessionId.Data);

    public static implicit operator byte[](SessionId sessionId) =>
        [..sessionId.Data];

    public static implicit operator ReadOnlySpan<byte>(SessionId sessionId) =>
        (byte[])sessionId;

    public override bool Equals(object? obj) =>
        obj is SessionId anotherSessionId
        && Data.SequenceEqual(anotherSessionId.Data);

    public override int GetHashCode() =>
        BitConverter.ToInt32(Data, 0);
}
