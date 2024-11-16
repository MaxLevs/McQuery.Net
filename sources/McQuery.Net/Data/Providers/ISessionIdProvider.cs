namespace McQuery.Net.Data.Providers;

/// <summary>
/// Provides <see cref="SessionId"/> every time it's needed.
/// </summary>
internal interface ISessionIdProvider
{
    /// <summary>
    /// Gets <see cref="SessionId"/>.
    /// </summary>
    /// <returns><see cref="SessionId"/>.</returns>
    SessionId Get();
}
