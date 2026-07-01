using ConsoleStarter;
using ConsoleStarter.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

var AppMeter = new System.Diagnostics.Metrics.Meter("EP.ConsoleStarter", "1.0.0");
var ItemsProcessed = AppMeter.CreateCounter<long>("items.processed", description: "Number of items processed");

// ============================================================================
// Модуль 6 — 🎯 Baseline Console Pipeline
// Финальная оркестрация: config → options → DI → build → run → shutdown → log
// ============================================================================

var builder = Host.CreateApplicationBuilder(args);

// ----------------------------------------------------------------------------
// 1. КОНФИГУРАЦИЯ (должна быть готова ДО регистрации сервисов). Порядок: JSON 
// → ENV → CLI (последний побеждает) (убрать в финале, потому что CreateApplicationBuilder это сам делает)
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
// 7. Модуль 7: Логирование через OTel
// ----------------------------------------------------------------------------

builder.Logging.ClearProviders();

builder.Logging.AddConsole();

// ----------------------------------------------------------------------------
// 8. OTEl (подключение трейсов и метрик)
// ----------------------------------------------------------------------------
builder.Logging.AddOpenTelemetry(logging =>
{
    logging.AddConsoleExporter();
});

builder.Services.AddOpenTelemetry()
    .WithTracing(t =>
        {
            t.AddSource("EP.ConsoleStarter");
            t.SetSampler(new AlwaysOnSampler());
            t.AddConsoleExporter();
            t.AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri("http://localhost:4317");
                options.Protocol = OtlpExportProtocol.HttpProtobuf;
            });
        })
    .WithMetrics(m =>
        {
            m.AddMeter("EP.ConsoleStarter");
            m.AddConsoleExporter();
            m.AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri("http://localhost:4317");
                options.Protocol = OtlpExportProtocol.HttpProtobuf;
            });
        });

using var activitySource = new System.Diagnostics.ActivitySource("EP.ConsoleStarter", "1.0.0");

// ----------------------------------------------------------------------------
// 4. BUILD (материализация хоста и контейнера)
// ----------------------------------------------------------------------------
var host = builder.Build();


// ----------------------------------------------------------------------------
// 5. ДЕМОНСТРАЦИЯ (резолв работает ТОЛЬКО после Build())
// ----------------------------------------------------------------------------

// 5.1: IConfiguration (string-значения)
// Console.WriteLine("\n=== Config: IConfiguration ===");
// Console.WriteLine($"Name: {builder.Configuration["App:Name"]}");
// Console.WriteLine($"Timeout: {builder.Configuration["App:Timeout"]} (Type: string)");

// // 5.2: IOptions<T> (статичный снимок, автоматическое приведение типов)
var settings = host.Services.GetRequiredService<IOptions<AppSettings>>().Value;
// Console.WriteLine("\n=== Config: IOptions<T> ===");
// Console.WriteLine($"Name: {settings.Name} (Type: {settings.Name.GetType().Name})");
// Console.WriteLine($"Timeout: {settings.Timeout} (Type: {settings.Timeout.GetType().Name})");

// 5.3: IOptionsMonitor<T> (реактивный доступ)
var monitorService = host.Services.GetRequiredService<ConfigMonitorService>();
monitorService.PrintCurrentConfig();
// модуль 7

var logger = host.Services.GetRequiredService<ILogger<Program>>();

using var scope = logger.BeginScope(new { TransactionId = Guid.NewGuid().ToString("N") });

// logger.LogInformation("Step about scope: Starting");
// logger.LogInformation("Step about scope: Processing");
// logger.LogInformation("Step about scope: Completed");

// logger.LogInformation("User {UserId} from {Ip} accessed {Resource}", 123, "10.0.0.1", "/api/data");

// logger.LogTrace("Trace message");
// logger.LogInformation("Information message");
// logger.LogWarning("Warning message");
// logger.LogError("Error message");

// ----------------------------------------------------------------------------
// 6. ЗАПУСК ХОСТА + ГЛОБАЛЬНАЯ ОБРАБОТКА ОШИБОК (Задача 6.3)
// ----------------------------------------------------------------------------
try
{
    await host.StartAsync();
    // При штатной остановке (включая остановку из-за ошибки в BackgroundService)
    // ExitCode остаётся 0 — это ожидаемое поведение для Generic Host
    using (var activity = activitySource.StartActivity("ProcessData"))
    {
        activity?.SetTag("item.count", 10);

        logger.LogInformation("Processing started with item count {ItemCount}", 10);
        logger.LogInformation("Processing completed");

        // Записываем метрику
        ItemsProcessed.Add(1, new KeyValuePair<string, object?>("item.type", "report"));

        await Task.Delay(100);
    }

    // Блокируемся до остановки
    await host.WaitForShutdownAsync();
}
catch (Exception ex) when (ex is not OperationCanceledException)
{
    // Ловим ТОЛЬКО ошибки инициализации (до RunAsync) или критичные сбои хоста
    Console.Error.WriteLine("\n🚨 CRITICAL ERROR: Application failed to run.");
    Console.Error.WriteLine($"Message: {ex.Message}");

    Directory.CreateDirectory("logs");
    var logPath = $"logs/crash-report-{DateTime.Now:yyyyMMdd-HHmmss}.txt";
    File.WriteAllText(logPath, $"[{DateTime.Now:O}]\n{ex}");
    Console.Error.WriteLine($"📄 Crash report saved to: {logPath}");

    Environment.ExitCode = 1;
}
