namespace MCQueryLib.Data.Packages
{
	public class Request
	{
		public byte[] RawRequestData { get; private set; }
		public byte RequestType => RawRequestData[2];

		public Request(byte[] rawRequestData)
		{
			RawRequestData = rawRequestData;
		}
	}
}
