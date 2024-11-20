using System.Net;
using McQuery.Net.Internal.Data;

namespace McQuery.Net.Internal.Abstract;

/// <summary>
/// Client that provides interface to acquire <see cref="ChallengeToken"/>.
/// </summary>
internal interface IAuthOnlyClient : IDisposable
{
    /// <summary>
    /// Request <see cref="ChallengeToken"/> from Minecraft server.
    /// </summary>
    /// <param name="serverEndpoint"></param>
    /// <param name="sessionId"><see cref="SessionId"/>.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns><see cref="ChallengeToken"/>.</returns>
    internal Task<ChallengeToken> HandshakeAsync(
        IPEndPoint serverEndpoint,
        SessionId sessionId,
        CancellationToken cancellationToken = default
    );
}
