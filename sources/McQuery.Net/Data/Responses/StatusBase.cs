namespace McQuery.Net.Data.Responses;

[PublicAPI]
public record StatusBase(
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
