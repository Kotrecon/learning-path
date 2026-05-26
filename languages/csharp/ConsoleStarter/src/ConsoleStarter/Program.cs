using ConsoleStarter.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Создаём билдер
var builder = Host.CreateApplicationBuilder(args);

// Регистрация сервисов
builder.Services.AddSingleton<ILoggerService, ConsoleLogger>();
builder.Services.AddScoped<IDataProcessor, DataProcessor>();
builder.Services.AddTransient<ITransientService, EphemeralService>();

// Строим хост
var host = builder.Build();

// Проверка резолва
Console.WriteLine("\n=== Проверка резолва ===");
var logger = host.Services.GetRequiredService<ILoggerService>();
logger.Log("Тест Singleton-резолва");

Console.WriteLine("\n=== Тест Scoped lifecycle ===");
var scopeFactory = host.Services.GetRequiredService<IServiceScopeFactory>();

for (int i = 1; i <= 3; i++)
{
    Console.WriteLine($"\n--- Итерация {i} ---");
    using var scope = scopeFactory.CreateScope();
    var processor = scope.ServiceProvider.GetRequiredService<IDataProcessor>();
    processor.Process($"Данные #{i}");
}

Console.WriteLine("\n=== Тест Transient lifecycle ===");
for (int i = 1; i <= 3; i++)
{
    Console.WriteLine($"\n--- Transient #{i} ---");
    var ephemeral = host.Services.GetRequiredService<ITransientService>();
    ephemeral.DoWork($"Операция #{i}");
    // Обратите внимание: НЕТ using, НЕТ ручного Dispose
}

// Запуск хоста
await host.RunAsync();

