namespace McQuery.Net;

/// <summary>
/// Factory to create instances of <see cref="IMcQueryClient"/>.
/// </summary>
[PublicAPI]
public interface IMcQueryClientFactory
{
    /// <summary>
    /// Create instance of <see cref="IMcQueryClient"/>.
    /// </summary>
    /// <returns>Instance of <see cref="IMcQueryClient"/>.</returns>
    IMcQueryClient Get();
}
