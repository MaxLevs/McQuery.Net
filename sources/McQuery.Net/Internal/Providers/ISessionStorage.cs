using System.Net;
using McQuery.Net.Internal.Data;

namespace McQuery.Net.Internal.Providers;

/// <summary>
/// Creates and stores <see cref="Session"/> objects.
/// </summary>
internal interface ISessionStorage : IDisposable
{
    /// <summary>
    /// Gets stored session or acquire new.
    /// </summary>
    /// <param name="serverEndpoint"><see cref="IPEndPoint"/> to access Minecraft server by UDP.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns><see cref="Session"/>.</returns>
    Task<Session> GetAsync(IPEndPoint serverEndpoint, CancellationToken cancellationToken = default);
}
