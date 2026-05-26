using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args).Build();


// Регистрация сервисов
builder.Services.AddSingleton<ILoggerService, ConsoleLogger>();
builder.Services.AddTransient<IDataProcessor, DataProcessor>();

// Пример для Singleton/Transient
var logger = host.Services.GetRequiredService<ILoggerService>();
// Пример для Scoped — только через скоуп!
using var scope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
var processor = scope.ServiceProvider.GetRequiredService<IDataProcessor>();

await host.RunAsync();
