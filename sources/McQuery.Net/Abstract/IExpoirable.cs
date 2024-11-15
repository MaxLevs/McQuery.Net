namespace McQuery.Net.Abstract;

internal interface IExpirable
{
    public bool IsExpired { get; }
}