namespace McQuery.Net.Data;

internal record Session(SessionId SessionId, ChallengeToken Token)
{
    public bool IsExpired => Token.IsExpired;
}
