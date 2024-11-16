using McQuery.Net.Data;
using McQuery.Net.Data.Packages;

namespace McQuery.Net.Services;

/// <summary>
/// This class builds Minecraft Query Packages for requests
/// Wiki: https://wiki.vg/Query
/// </summary>
public static class RequestFormingService
{
    private static readonly byte[] MagicConst = { 0xfe, 0xfd };

    private static readonly byte[] ChallengeRequestConst = { 0x09 };

    private static readonly byte[] StatusRequestConst = { 0x00 };

    public static Request HandshakeRequestPackage(SessionId sessionId)
    {
        List<byte> data = new(224);
        data.AddRange(MagicConst);
        data.AddRange(ChallengeRequestConst);
        sessionId.WriteTo(data);

        Request request = new(data.ToArray());

        return request;
    }

    public static Request GetBasicStatusRequestPackage(SessionId sessionId, ChallengeToken challengeToken)
    {
        if (challengeToken == null) throw new ChallengeTokenIsNullException();

        List<byte> data = new(416);
        data.AddRange(MagicConst);
        data.AddRange(StatusRequestConst);
        sessionId.WriteTo(data);
        challengeToken.WriteTo(data);

        Request request = new(data.ToArray());

        return request;
    }

    public static Request GetFullStatusRequestPackage(SessionId sessionId, ChallengeToken challengeToken)
    {
        if (challengeToken == null) throw new ChallengeTokenIsNullException();

        List<byte> data = new(544);
        data.AddRange(MagicConst);
        data.AddRange(StatusRequestConst);
        sessionId.WriteTo(data);
        challengeToken.WriteTo(data);
        data.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00 }); // Padding

        Request request = new(data.ToArray());

        return request;
    }
}

public class ChallengeTokenIsNullException : Exception
{
    public ChallengeTokenIsNullException()
    {
    }

    public ChallengeTokenIsNullException(string? message) : base(message)
    {
    }

    public ChallengeTokenIsNullException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
