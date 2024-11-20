# McQuery.Net

Library for .Net which implements Minecraft Query protocol. You can use it for getting statuses of a Minecraft server.

# Example of using

```cs
IMcQueryClientFactory factory = new McQueryClientFactory();
using var client = factory.Get();

async Task ExecuteQueries(IReadOnlyCollection<IPEndPoint> endpoints, CancellationToken cancellationToken = default)
{
    var queryTasks = endpoints.SelectMany<IPEndPoint, Task, Task>(
        endpoint =>
        [
            GetBasicStatusAndPrint(endpoint, cancellationToken),
            GetFullStatusAndPrint(endpoint, cancellationToken)
        ],
        (_, task) => task
    ).ToArray();

    await Task.WhenAll(queryTasks);
}

async Task GetBasicStatusAndPrint(IPEndPoint endpoint, CancellationToken cancellationToken = default)
{
    Console.WriteLine(await client.GetBasicStatusAsync(endpoint, cancellationToken));
}

async Task GetFullStatusAndPrint(IPEndPoint endpoint, CancellationToken cancellationToken = default)
{
    Console.WriteLine(await client.GetFullStatusAsync(endpoint, cancellationToken));
}
```
