using System.Diagnostics;
using System.Net;
using McQuery.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var serviceProvider = new ServiceCollection()
    .AddLogging(
        builder =>
        {
            builder.SetMinimumLevel(LogLevel.Information);
            builder.AddConsole();
        })
    .AddSingleton<IMcQueryClientFactory, McQueryClientFactory>()
    .BuildServiceProvider();

var factory = serviceProvider.GetRequiredService<IMcQueryClientFactory>();
using var client = factory.Get();

int[] ports = [25565, 25566, 25567];
Func<IPEndPoint, CommandBase>[] commandFactories =
[
    ep => new BasicStatusCommand(ep),
    ep => new FullStatusCommand(ep),
];
CommandBase[] commands =
[
    ..
    from _ in Enumerable.Range(start: 0, count: 5000)
    from fc in commandFactories
    from port in ports
    select fc(new IPEndPoint(IPAddress.Loopback, port)),
];
Random.Shared.Shuffle(commands);

var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
logger.IsEnabled(LogLevel.Trace);
logger.LogInformation("Starting McQuery.Net.Sample with {Count} requests", commands.Length);
var stopwatch = Stopwatch.StartNew();
await Task.WhenAll(commands.Select(x => x.ExecuteAsync(client)).ToArray());
stopwatch.Stop();
logger.LogInformation("Finished. It took {Elapsed}", stopwatch.Elapsed);

public abstract class CommandBase(IPEndPoint endPoint)
{
    public abstract Task ExecuteAsync(IMcQueryClient client, CancellationToken cancellationToken = default);
}

public class BasicStatusCommand(IPEndPoint endPoint) : CommandBase(endPoint)
{
    private readonly IPEndPoint _endPoint = endPoint;

    public override Task ExecuteAsync(IMcQueryClient client, CancellationToken cancellationToken = default) =>
        client.GetBasicStatusAsync(_endPoint, cancellationToken);
}

public class FullStatusCommand(IPEndPoint endPoint) : CommandBase(endPoint)
{
    private readonly IPEndPoint _endPoint = endPoint;

    public override Task ExecuteAsync(IMcQueryClient client, CancellationToken cancellationToken = default) =>
        client.GetFullStatusAsync(_endPoint, cancellationToken);
}
