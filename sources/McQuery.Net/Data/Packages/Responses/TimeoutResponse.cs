using System;

namespace MCQueryLib.Data.Packages.Responses
{
	public class TimeoutResponse : IResponse
	{
		public TimeoutResponse(Guid serverUUID)
		{
			ServerUUID = serverUUID;
		}

		public byte[] RawData => throw new NotSupportedException();
		public Guid ServerUUID { get; }
		public string Message => "Request is timed out";
	}
}
