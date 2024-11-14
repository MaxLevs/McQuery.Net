using System;

namespace MCQueryLib.Data.Packages.Responses
{
	public class RawResponse : IResponse
	{
		public RawResponse(Guid serverUUID, byte[] rawData)
		{
			ServerUUID = serverUUID;
			RawData = rawData;
		}

		public Guid ServerUUID { get; }
		public byte[] RawData { get; }
	}
}
