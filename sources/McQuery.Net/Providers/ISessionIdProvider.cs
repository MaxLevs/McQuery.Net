using McQuery.Net.Data;

namespace McQuery.Net.Providers;

internal interface ISessionIdProvider
{
    SessionId Get();
}
