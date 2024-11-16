using System.Net;
using McQuery.Net.Data;

namespace McQuery.Net.Abstract;

internal interface IAuthOnlyClient : IDisposable
{

    internal Task<ChallengeToken> HandshakeAsync(
        IPEndPoint serverEndpoint,
        SessionId sessionId,
        CancellationToken cancellationToken = default);
}
