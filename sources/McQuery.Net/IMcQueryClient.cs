using System.Net;
using McQuery.Net.Data.Responses;

namespace McQuery.Net;

[PublicAPI]
public interface IMcQueryClient
{
    Task<BasicStatus> GetBasicStatusAsync(IPEndPoint serverEndPoint, CancellationToken cancellationToken = default);
    BasicStatus GetBasicStatus(IPEndPoint serverEndPoint, CancellationToken cancellationToken = default);

    Task<FullStatus> GetFullStatusAsync(IPEndPoint serverEndPoint, CancellationToken cancellationToken = default);
    FullStatus GetFullStatus(IPEndPoint serverEndPoint, CancellationToken cancellationToken = default);
}
