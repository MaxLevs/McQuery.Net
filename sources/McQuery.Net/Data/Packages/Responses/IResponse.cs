namespace McQuery.Net.Data.Packages.Responses;

public interface IResponse
{
    public Guid ServerUUID { get; }

    public byte[] RawData { get; }
}
