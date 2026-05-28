using ConsoleStarter;
using ConsoleStarter.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;


// ============================================================================
// Модуль 4 — Options & Runtime Reconfiguration
// ============================================================================

var builder = Host.CreateApplicationBuilder(args);

// Явно включаем отслеживание изменений файла
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// ----------------------------------------------------------------------------
// Биндинг типизированных настроек
// ----------------------------------------------------------------------------
// Было:
// builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("App"));

// Стало (с валидацией):
builder.Services
    .AddOptions<AppSettings>()
    .BindConfiguration("App")
    .ValidateDataAnnotations()                // ← Проверяем атрибуты
    .ValidateOnStart();                       // ← Fail-fast при старте

// Регистрируем сервис-монитор (Singleton)
builder.Services.AddSingleton<ConfigMonitorService>();

var host = builder.Build();

// ----------------------------------------------------------------------------
// Демонстрация: чтение конфига разными способами
// ----------------------------------------------------------------------------

// 1. Через IConfiguration (возвращает string)
Console.WriteLine("\n=== Config: IConfiguration ===");
Console.WriteLine($"Name: {builder.Configuration["App:Name"]}");
Console.WriteLine($"Timeout: {builder.Configuration["App:Timeout"]} (Type: string)");

// 2. Через IOptions<T> (статичный снимок, автоматическое приведение типов)
var settings = host.Services.GetRequiredService<IOptions<AppSettings>>().Value;
Console.WriteLine("\n=== Config: IOptions<T> ===");
Console.WriteLine($"Name: {settings.Name} (Type: {settings.Name.GetType().Name})");
Console.WriteLine($"Timeout: {settings.Timeout} (Type: {settings.Timeout.GetType().Name})");

// 3. Через IOptionsMonitor<T> (реактивный, актуальное значение)
var monitorService = host.Services.GetRequiredService<ConfigMonitorService>();
monitorService.PrintCurrentConfig();

// ----------------------------------------------------------------------------
// Запуск хоста: ждём сигнал остановки (Ctrl+C)
// ----------------------------------------------------------------------------
await host.RunAsync();