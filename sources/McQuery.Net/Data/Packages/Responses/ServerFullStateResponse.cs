using System;

namespace MCQueryLib.Data.Packages.Responses
{
	/// <summary>
	/// Represents data which is received from FullState request
	/// </summary>
	public class ServerFullStateResponse : IResponse
	{
		public ServerFullStateResponse(Guid serverUUID,
									   SessionId sessionId,
									   string motd,
									   string gameType,
									   string gameId,
									   string version,
									   string plugins,
									   string map,
									   int numPlayers,
									   int maxPlayers,
									   string[] playerList,
									   int hostPort,
									   string hostIp,
									   byte[] rawData)
		{
			ServerUUID = serverUUID;
			SessionId = sessionId;
			Motd = motd;
			GameType = gameType;
			GameId = gameId;
			Version = version;
			Plugins = plugins;
			Map = map;
			NumPlayers = numPlayers;
			MaxPlayers = maxPlayers;
			PlayerList = playerList;
			HostPort = hostPort;
			HostIp = hostIp;
			RawData = rawData;
		}

		public SessionId SessionId { get; }

		public string Motd { get; }
		public string GameType { get; }
		public string GameId { get; }
		public string Version { get; }
		public string Plugins { get; }
		public string Map { get; }
		public int NumPlayers { get; }
		public int MaxPlayers { get; }
		public string[] PlayerList { get; }
		public int HostPort { get; }
		public string HostIp { get; }

		public Guid ServerUUID { get; }
		public byte[] RawData { get; }

		public override string ToString()
		{
			return "FullStatus\n" +
				   $"| {nameof(ServerUUID)}: {ServerUUID}\n" +
				   $"| {nameof(SessionId)}: {SessionId.GetString()}\n" +
				   $"| {nameof(Motd)}: {Motd}\n" +
				   $"| {nameof(GameType)}: {GameType}\n" +
				   $"| {nameof(GameId)}: {GameId}\n" +
				   $"| {nameof(Version)}: {Version}\n" +
				   $"| {nameof(Plugins)}: {Plugins}\n" +
				   $"| {nameof(Map)}: {Map}\n" +
				   $"| {nameof(NumPlayers)}: {NumPlayers}\n" +
				   $"| {nameof(MaxPlayers)}: {MaxPlayers}\n" +
				   $"| {nameof(PlayerList)}: [{string.Join(", ", PlayerList)}]\n" +
				   $"| {nameof(HostPort)}: {HostPort}\n" +
				   $"| {nameof(HostIp)}: {HostIp}";
		}
	}
}