using ConsoleStarter;
using ConsoleStarter.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

// ============================================================================
// Модуль 6 — 🎯 Baseline Console Pipeline
// Финальная оркестрация: config → options → DI → build → run → shutdown → log
// ============================================================================

var builder = Host.CreateApplicationBuilder(args);

// ----------------------------------------------------------------------------
// 1. КОНФИГУРАЦИЯ (должна быть готова ДО регистрации сервисов). Порядок: JSON 
// → ENV → CLI (последний побеждает)
// ----------------------------------------------------------------------------
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args);

// ----------------------------------------------------------------------------
// 2. ОПЦИИ (валидация на основе загруженной конфигурации)
// ----------------------------------------------------------------------------
builder.Services
    .AddOptions<AppSettings>()
    .BindConfiguration("App")
    .ValidateDataAnnotations()
    .ValidateOnStart();

// ----------------------------------------------------------------------------
// 3. РЕГИСТРАЦИЯ СЕРВИСОВ (DI-контейнер строится на основе конфига)
// ----------------------------------------------------------------------------
// Модуль 4: реактивный мониторинг конфига
builder.Services.AddSingleton<ConfigMonitorService>();

// Модуль 5: фоновый воркер с кооперативной отменой
builder.Services.AddHostedService<PipelineWorker>();

// ----------------------------------------------------------------------------
// 4. BUILD (материализация хоста и контейнера)
// ----------------------------------------------------------------------------
var host = builder.Build();

// ----------------------------------------------------------------------------
// 5. ДЕМОНСТРАЦИЯ (резолв работает ТОЛЬКО после Build())
// ----------------------------------------------------------------------------

// 5.1: IConfiguration (string-значения)
Console.WriteLine("\n=== Config: IConfiguration ===");
Console.WriteLine($"Name: {builder.Configuration["App:Name"]}");
Console.WriteLine($"Timeout: {builder.Configuration["App:Timeout"]} (Type: string)");

// 5.2: IOptions<T> (статичный снимок, автоматическое приведение типов)
var settings = host.Services.GetRequiredService<IOptions<AppSettings>>().Value;
Console.WriteLine("\n=== Config: IOptions<T> ===");
Console.WriteLine($"Name: {settings.Name} (Type: {settings.Name.GetType().Name})");
Console.WriteLine($"Timeout: {settings.Timeout} (Type: {settings.Timeout.GetType().Name})");

// 5.3: IOptionsMonitor<T> (реактивный доступ)
var monitorService = host.Services.GetRequiredService<ConfigMonitorService>();
monitorService.PrintCurrentConfig();

// ----------------------------------------------------------------------------
// 6. ЗАПУСК ХОСТА (блокирует поток до сигнала остановки)
// ----------------------------------------------------------------------------
await host.RunAsync();