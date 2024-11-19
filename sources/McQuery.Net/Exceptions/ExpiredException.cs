using McQuery.Net.Internal.Abstract;

namespace McQuery.Net.Exceptions;

[PublicAPI]
public class ExpiredException : ArgumentException
{
    internal ExpiredException(IExpirable expirable)
        : base($"{expirable.GetType().Name} is already expired")
    {
    }

    internal static void ThrowIfExpired(IExpirable expirable)
    {
        if (expirable.IsExpired) throw new ExpiredException(expirable);
    }
}
