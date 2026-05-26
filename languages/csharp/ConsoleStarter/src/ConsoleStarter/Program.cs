using ConsoleStarter.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Создаём билдер
var builder = Host.CreateApplicationBuilder(args);

// Регистрация сервисов
builder.Services.AddSingleton<ILoggerService, ConsoleLogger>();
builder.Services.AddScoped<IDataProcessor, DataProcessor>();

// Строим хост
var host = builder.Build();

// Проверка резолва
Console.WriteLine("\n=== Проверка резолва ===");
var logger = host.Services.GetRequiredService<ILoggerService>();
logger.Log("Тест Singleton-резолва");

// Запуск хоста
await host.RunAsync();

