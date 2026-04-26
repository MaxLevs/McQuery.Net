namespace McQuery.Net.Internal.Data;

/// <summary>
/// Represents a combination of <see cref="SessionId"/> and <see cref="ChallengeToken"/> values.
/// </summary>
/// <remarks>
/// Replica of something similar that Minecraft server use.
/// </remarks>
/// <param name="SessionId"><see cref="SessionId"/>.</param>
/// <param name="Token"><see cref="ChallengeToken"/>.</param>
internal record Session(SessionId SessionId, ChallengeToken Token)
{
    public bool IsExpired => Token.IsExpired;
}
