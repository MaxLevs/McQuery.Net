using McQuery.Net.Internal.Data;

namespace McQuery.Net.Internal.Providers;

/// <summary>
/// Implementation of <see cref="IServiceProvider"/>.
/// </summary>
internal class SessionIdProvider : ISessionIdProvider
{
    private static uint counter;

    /// <inheritdoc />
    public SessionId Get()
    {
        var currentValue = Interlocked.Increment(ref counter);

        var bytes = BitConverter.GetBytes(currentValue);
        if (BitConverter.IsLittleEndian) Array.Reverse(bytes);

        return new SessionId(bytes);
    }
}
