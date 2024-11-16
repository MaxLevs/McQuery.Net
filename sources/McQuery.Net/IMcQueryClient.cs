using System.Net;
using McQuery.Net.Data.Responses;

namespace McQuery.Net;

[PublicAPI]
public interface IMcQueryClient : IDisposable
{
    Task<BasicStatus> GetBasicStatusAsync(IPEndPoint serverEndpoint, CancellationToken cancellationToken = default);
    BasicStatus GetBasicStatus(IPEndPoint serverEndpoint, CancellationToken cancellationToken = default);

    Task<FullStatus> GetFullStatusAsync(IPEndPoint serverEndpoint, CancellationToken cancellationToken = default);
    FullStatus GetFullStatus(IPEndPoint serverEndpoint, CancellationToken cancellationToken = default);
}
