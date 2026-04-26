namespace McQuery.Net.Exceptions;

/// <summary>
/// Response parsing exception.
/// </summary>
[PublicAPI]
public class McQueryResponseParsingException : McQueryException
{
    /// <summary>
    /// Initializes new instance of basic exception.
    /// </summary>
    internal McQueryResponseParsingException()
    {
    }

    /// <summary>
    /// Initializes new instance of basic exception.
    /// </summary>
    internal McQueryResponseParsingException(string? message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes new instance of basic exception.
    /// </summary>
    internal McQueryResponseParsingException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
