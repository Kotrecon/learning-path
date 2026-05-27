using ConsoleStarter;
using ConsoleStarter.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

// ============================================================================
// Модуль 3 — Конфигурация
// ============================================================================

var builder = Host.CreateApplicationBuilder(args);

// Явно добавляем JSON с поддержкой перезагрузки
// Это перекроет дефолтный провайдер и включит отслеживание изменений файла
// При reloadOnChange: false (умолчанию) данная команда не требуется
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);


// ----------------------------------------------------------------------------
// 3.1: Базовое чтение из appsettings.json (через IConfiguration)
// Требования:
//   • Файл appsettings.json в проекте с <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
//   • Разделитель вложенности — двоеточие :  (App:Timeout)
//   • Все значения приходят как string, приведение типов — вручную
// ----------------------------------------------------------------------------
Console.WriteLine("\n=== Config: JSON (IConfiguration) ===");
Console.WriteLine($"Name: {builder.Configuration["App:Name"]}");
Console.WriteLine($"Timeout: {builder.Configuration["App:Timeout"]} (Type: string)");

// ----------------------------------------------------------------------------
// 3.5: Типизированный доступ через IOptions<T>
// Дополнительные требования (сверх 3.1):
//   • Record/класс с параметрless конструктором (init-свойства, не позиционные)
//   • Регистрация: builder.Services.Configure<T>(section)
//   • Резолв: host.Services.GetRequiredService<IOptions<T>>().Value
//   • Биндинг происходит при первом .Value, затем кэшируется как Singleton
//   • Автоматическое приведение типов: "30" → int 30
// ----------------------------------------------------------------------------
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("App"));

builder.Services.AddSingleton<ConfigMonitorService>();

var host = builder.Build();

var settings = host.Services.GetRequiredService<IOptions<AppSettings>>().Value;
Console.WriteLine("\n=== Config: IOptions<T> Binding ===");
Console.WriteLine($"Name: {settings.Name} (Type: {settings.Name.GetType().Name})");
Console.WriteLine($"Timeout: {settings.Timeout} (Type: {settings.Timeout.GetType().Name})");

// === Модуль 4.1: Реактивный доступ через IOptionsMonitor ===
var monitorService = host.Services.GetRequiredService<ConfigMonitorService>();
monitorService.PrintCurrentConfig(); // Покажет актуальное значение (при старте или после изменения)

// === Запуск хоста ===
// Для продакшена: ждём сигнал остановки (Ctrl+C)

// ----------------------------------------------------------------------------
// Запуск/остановка хоста
// Для тестов: StartAsync + StopAsync (процесс завершается сам)
// Для прода:   await host.RunAsync() (ждёт Ctrl+C / SIGTERM)
// ----------------------------------------------------------------------------
// await host.StartAsync();
// await host.StopAsync();

await host.RunAsync();