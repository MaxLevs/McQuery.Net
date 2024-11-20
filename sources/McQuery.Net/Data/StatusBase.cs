using McQuery.Net.Internal.Data;

namespace McQuery.Net.Data;

/// <summary>
/// Represents a basic status response.
/// </summary>
/// <seealso href="https://wiki.vg/Query"/>
/// <param name="Motd">Message of the day.</param>
/// <param name="GameType">Type of the game.</param>
/// <param name="Map">Name of a map.</param>
/// <param name="NumPlayers">Current number of players.</param>
/// <param name="MaxPlayers">Maximum number of players what is allowed to enter.</param>
/// <param name="HostPort">Port to connect.</param>
/// <param name="HostIp">Ip to connect.</param>
[PublicAPI]
public abstract record StatusBase(
    string Motd,
    string GameType,
    string Map,
    int NumPlayers,
    int MaxPlayers,
    int HostPort,
    string HostIp)
{
    internal SessionId SessionId { get; init; } = null!;
}
