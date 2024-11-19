using System.Net;
using McQuery.Net.Data;

namespace McQuery.Net;

/// <summary>
/// Client to request minecraft server status by Minecraft Query Protocol.
/// </summary>
[PublicAPI]
public interface IMcQueryClient : IDisposable
{
    /// <summary>
    /// Get <see cref="BasicStatus"/>.
    /// </summary>
    /// <param name="serverEndpoint"><see cref="IPEndPoint"/> to access Minecraft server by UDP.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns><see cref="BasicStatus"/>.</returns>
    Task<BasicStatus> GetBasicStatusAsync(IPEndPoint serverEndpoint, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="GetBasicStatusAsync"/>
    BasicStatus GetBasicStatus(IPEndPoint serverEndpoint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get <see cref="FullStatus"/>.
    /// </summary>
    /// <remarks>
    /// Minecraft server caches prepared full status response for some time.
    /// </remarks>
    /// <param name="serverEndpoint"><see cref="IPEndPoint"/> to access Minecraft server by UDP.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns><see cref="FullStatus"/>.</returns>
    Task<FullStatus> GetFullStatusAsync(IPEndPoint serverEndpoint, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="GetFullStatusAsync"/>
    FullStatus GetFullStatus(IPEndPoint serverEndpoint, CancellationToken cancellationToken = default);
}
