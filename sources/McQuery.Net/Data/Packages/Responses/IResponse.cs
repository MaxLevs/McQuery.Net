using System;

namespace MCQueryLib.Data.Packages.Responses
{
	public interface IResponse
	{
		public Guid ServerUUID { get; }
		public byte[] RawData { get; }
	}
}
