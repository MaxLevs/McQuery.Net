namespace McQuery.Net.Data.Factories;

/// <summary>
/// Provides methods to build requests.
/// </summary>
internal interface IRequestFactory
{
    /// <summary>
    /// Builds handshake request.
    /// </summary>
    /// <param name="sessionId"><see cref="SessionId"/>.</param>
    /// <returns>Binary representation of the request.</returns>
    internal byte[] GetHandshakeRequest(SessionId sessionId);

    /// <summary>
    /// Builds basic status request.
    /// </summary>
    /// <param name="session"><see cref="Session"/>.</param>
    /// <returns>Binary representation of the request.</returns>
    internal byte[] GetBasicStatusRequest(Session session);

    /// <summary>
    /// Builds full status request.
    /// </summary>
    /// <param name="session"><see cref="Session"/>.</param>
    /// <returns>Binary representation of the request.</returns>
    internal byte[] GetFullStatusRequest(Session session);
}
