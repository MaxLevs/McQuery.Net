namespace McQuery.Net.Data.Factories;

/// <summary>
/// Implementation of <see cref="IRequestFactory"/>.
/// </summary>
internal class RequestFactory : IRequestFactory
{
    private const byte HandshakeRequestTypeConst = 0x09;
    private const byte StatusRequestTypeConst = 0x00;
    private static readonly byte[] MagicConst = [0xfe, 0xfd];

    /// <inheritdoc />
    public byte[] GetHandshakeRequest(SessionId sessionId)
    {
        using MemoryStream packetStream = new();
        FormRequestHeader(packetStream, HandshakeRequestTypeConst, sessionId);
        return packetStream.ToArray();
    }

    /// <inheritdoc />
    public byte[] GetBasicStatusRequest(Session session)
    {
        using MemoryStream packetStream = new();
        FormBasicStatusRequest(packetStream, session);
        return packetStream.ToArray();
    }

    /// <inheritdoc />
    public byte[] GetFullStatusRequest(Session session)
    {
        using MemoryStream packetStream = new();
        FormBasicStatusRequest(packetStream, session);
        packetStream.Write([0x00, 0x00, 0x00, 0x00]);
        return packetStream.ToArray();
    }

    private static void FormRequestHeader(Stream packetStream, byte packageType, SessionId sessionId)
    {
        packetStream.Write(MagicConst);
        packetStream.Write([packageType]);
        packetStream.Write(sessionId);
    }

    private static void FormBasicStatusRequest(Stream packetStream, Session session)
    {
        FormRequestHeader(packetStream, StatusRequestTypeConst, session.SessionId);
        packetStream.Write(session.Token);
    }
}
