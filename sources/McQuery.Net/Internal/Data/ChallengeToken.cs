using McQuery.Net.Exceptions;
using McQuery.Net.Internal.Abstract;

namespace McQuery.Net.Internal.Data;

/// <summary>
/// Secret value provided by Minecraft server to issue status requests.
/// </summary>
internal record ChallengeToken : IExpirable
{
    private static readonly TimeSpan alivePeriod = TimeSpan.FromSeconds(30);
    private readonly DateTimeOffset _expiresAt = DateTimeOffset.UtcNow.Add(alivePeriod);

    /// <summary>
    /// .ctor.
    /// </summary>
    /// <param name="data">Bytes that represents challenge token.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Number of bytes is incorrect.
    /// </exception>
    public ChallengeToken(byte[] data)
    {
        ValidateHave4Bytes(data);

        Data = data;
    }

    private byte[] Data { get; }
    public bool IsExpired => DateTimeOffset.UtcNow >= _expiresAt;

    public static implicit operator byte[](ChallengeToken token)
    {
        McQueryExpiredException.ThrowIfExpired(token);
        return [..token.Data];
    }

    public static implicit operator ReadOnlySpan<byte>(ChallengeToken token)
        => (byte[])token;

    private static void ValidateHave4Bytes(byte[] data)
    {
        if (data.Length != 4)
        {
            throw new McQueryException("Challenge token must have 4 bytes");
        }
    }
}
