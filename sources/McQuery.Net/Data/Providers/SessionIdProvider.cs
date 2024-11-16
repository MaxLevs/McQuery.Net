namespace McQuery.Net.Data.Providers;

/// <summary>
/// Implementation of <see cref="IServiceProvider"/>.
/// </summary>
internal class SessionIdProvider : ISessionIdProvider
{
    private static uint counter;

    /// <inheritdoc />
    public SessionId Get()
    {
        uint currentValue = Interlocked.Increment(ref counter);

        byte[] bytes = BitConverter.GetBytes(currentValue);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }

        return new SessionId(bytes);
    }
}
