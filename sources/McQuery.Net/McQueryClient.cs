using System.Net;
using McQuery.Net.Data.Responses;

namespace McQuery.Net;

[PublicAPI]
public class McQueryClient : IMcQueryClient
{
    public async Task<BasicStatus> GetBasicStatusAsync(IPEndPoint serverEndPoint, CancellationToken cancellationToken = default)
    {
        await Task.Yield();
        throw new NotImplementedException();
    }

    public async Task<FullStatus> GetFullStatusAsync(IPEndPoint serverEndPoint, CancellationToken cancellationToken = default)
    {
        await Task.Yield();
        throw new NotImplementedException();
    }

    public BasicStatus GetBasicStatus(IPEndPoint serverEndPoint, CancellationToken cancellationToken = default) =>
        GetBasicStatusAsync(serverEndPoint, cancellationToken).GetAwaiter().GetResult();

    public FullStatus GetFullStatus(IPEndPoint serverEndPoint, CancellationToken cancellationToken = default) =>
        GetFullStatusAsync(serverEndPoint, cancellationToken).GetAwaiter().GetResult();
}
