namespace McQuery.Net.Data;

/// <summary>
/// Represents data which is received from FullStatus request.
/// </summary>
/// <seealso href="https://wiki.vg/Query#Response_3"/>
/// <param name="Motd">Message of the day.</param>
/// <param name="GameType">Type of the game.</param>
/// <param name="GameId">Identifier of a game. Constant value: MINECRAFT.</param>
/// <param name="Version">Game version number.</param>
/// <param name="Plugins">List of plugins as a string.</param>
/// <param name="Map">Name of a map.</param>
/// <param name="NumPlayers">Current number of players.</param>
/// <param name="MaxPlayers">Maximum number of players what is allowed to enter.</param>
/// <param name="PlayerList">List of players' nicknames.</param>
/// <param name="HostPort">Port to connect.</param>
/// <param name="HostIp">Ip to connect.</param>
[PublicAPI]
public record FullStatus(
    string Motd,
    string GameType,
    string GameId,
    string Version,
    string Plugins,
    string Map,
    int NumPlayers,
    int MaxPlayers,
    string[] PlayerList,
    int HostPort,
    string HostIp
) : StatusBase(
    Motd,
    GameType,
    Map,
    NumPlayers,
    MaxPlayers,
    HostPort,
    HostIp
);
