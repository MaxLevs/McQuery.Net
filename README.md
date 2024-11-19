# McQuery.Net
Library for .Net which implements Minecraft Query protocol. You can use it for getting statuses of a Minecraft server.

# Example of using

```cs
static async Task DoSomething(IEnumerable<IPEndPoint> mcServersEndPoints)
{
	McQueryService service = new(5, 5000, 500, 1000);

	List<Server> servers = mcServersEndPoints.Select(service.RegistrateServer).ToList();

	List<Task<IResponse>> requests = new();
	foreach (Server server in servers)
	{
		requests.Add(service.GetBasicStatusCommon(server));
		requests.Add(service.GetFullStatusCommon(server));
	}

	Task.WaitAll(requests.ToArray());

	foreach (Task<IResponse> request in requests)
	{
		IResponse response = await request;
		Console.WriteLine(response.ToString() + "\n");
	}
}
```
