using System.Runtime.Serialization;

namespace McQuery.Net.Exceptions;

/// <summary>
/// Basic class for all exceptions to this library.
/// </summary>
public abstract class McQueryException : Exception
{
    /// <inheritdoc />
    protected McQueryException()
    {
    }

    /// <inheritdoc />
    protected McQueryException(string? message) : base(message)
    {
    }

    /// <inheritdoc />
    protected McQueryException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
