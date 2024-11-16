using McQuery.Net.Abstract;
using McQuery.Net.Exceptions;

namespace McQuery.Net.Data;

internal record ChallengeToken : IExpirable
{
    private const int AlivePeriod = 29;
    private readonly DateTime expiresAt = DateTime.UtcNow.AddSeconds(AlivePeriod);

    /// <summary>
    /// .ctor.
    /// </summary>
    /// <param name="data">Bytes that represents challenge token.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Number of bytes is incorrect.
    /// </exception>
    public ChallengeToken(byte[] data)
    {
        if (data.Length != 4)
            throw new ArgumentOutOfRangeException(nameof(data), data, "Challenge token must have 4 bytes");

        Data = data;
    }

    private byte[] Data { get; }
    public bool IsExpired => DateTime.UtcNow >= expiresAt;

    public static implicit operator byte[](ChallengeToken token)
    {
        AlreadyExpiredException.ThrowIfExpired(token);
        return [..token.Data];
    }

    public static implicit operator ReadOnlySpan<byte>(ChallengeToken token) =>
        (byte[])token;
}
