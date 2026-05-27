using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// === Модуль 3.1: Чтение JSON ===
// builder.Configuration уже содержит appsettings.json
Console.WriteLine("\n=== Config: JSON ===");
Console.WriteLine($"Name: {builder.Configuration["App:Name"]}");
Console.WriteLine($"Timeout: {builder.Configuration["App:Timeout"]}");

var host = builder.Build();
await host.RunAsync();