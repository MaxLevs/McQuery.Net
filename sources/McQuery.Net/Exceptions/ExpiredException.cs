using McQuery.Net.Internal.Abstract;

namespace McQuery.Net.Exceptions;

/// <summary>
/// Something was expired.
/// </summary>
[PublicAPI]
public class ExpiredException : ArgumentException
{
    internal ExpiredException(IExpirable expirable)
        : base($"{expirable.GetType().Name} is already expired")
    {
    }

    /// <summary>
    /// Helper method to throw new exception form <see cref="IExpirable"/>.
    /// </summary>
    /// <param name="expirable">Something that can be expired.</param>
    /// <exception cref="ExpiredException">Something was exprired.</exception>
    internal static void ThrowIfExpired(IExpirable expirable)
    {
        if (expirable.IsExpired) throw new ExpiredException(expirable);
    }
}
