using System.Net;

namespace McQuery.Net.Data.Providers;

internal interface ISessionStorage: IDisposable
{
    Task<Session> GetAsync(IPEndPoint serverEndpoint, CancellationToken token = default);
}
