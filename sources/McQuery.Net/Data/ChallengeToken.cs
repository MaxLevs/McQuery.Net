namespace McQuery.Net.Data;

public class ChallengeToken
{
    private byte[]? challengeToken;

    private const int alivePeriod = 30000; // Milliseconds before revoking

    private DateTime revokeDateTime;

    public bool IsFine => challengeToken != null && DateTime.Now < revokeDateTime;

    public ChallengeToken()
    {
        challengeToken = null;
    }

    public ChallengeToken(byte[] challengeToken)
    {
        UpdateToken(challengeToken);
    }

    public void UpdateToken(byte[] challengeToken)
    {
        this.challengeToken = (byte[])challengeToken.Clone();
        revokeDateTime = DateTime.Now.AddMilliseconds(alivePeriod);
    }

    public string GetString()
    {
        ArgumentNullException.ThrowIfNull(challengeToken);

        return BitConverter.ToString(challengeToken);
    }

    public byte[] GetBytes()
    {
        ArgumentNullException.ThrowIfNull(challengeToken);

        byte[] challengeTokenSnapshot = new byte[4];
        Buffer.BlockCopy(challengeToken, 0, challengeTokenSnapshot, 0, 4);

        return challengeTokenSnapshot;
    }

    public void WriteTo(List<byte> list)
    {
        ArgumentNullException.ThrowIfNull(challengeToken);

        list.AddRange(challengeToken);
    }
}
