using System.Runtime.Serialization;

namespace McQuery.Net.Exceptions;

/// <summary>
/// Basic class for all exceptions to this library.
/// </summary>
[PublicAPI]
public class McQueryException : Exception
{
    /// <summary>
    /// Initializes new instance of basic exception.
    /// </summary>
    internal McQueryException()
    {
    }

    /// <summary>
    /// Initializes new instance of basic exception.
    /// </summary>
    internal McQueryException(string? message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes new instance of basic exception.
    /// </summary>
    internal McQueryException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
