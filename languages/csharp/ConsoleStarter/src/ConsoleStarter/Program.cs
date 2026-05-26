using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args).Build();


Console.WriteLine("Before RunAsync");
await host.RunAsync();
Console.WriteLine("After RunAsync");