using McQuery.Net.Data;

namespace McQuery.Net.Providers;

internal class SessionIdProvider : ISessionIdProvider
{
    private static uint counter;

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
