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
// Биндинг типизированных настроек с валидацией
// ----------------------------------------------------------------------------
builder.Services
    .AddOptions<AppSettings>()
    .BindConfiguration("App")
    .ValidateDataAnnotations()                // ← Проверяем атрибуты
    .ValidateOnStart();                       // ← Fail-fast при старте

// Регистрируем сервисы
builder.Services.AddSingleton<ConfigMonitorService>();
// builder.Services.AddHostedService<WorkerService>();
builder.Services.AddHostedService<PipelineWorker>();

var host = builder.Build();

// === Подписка на события жизненного цикла (Задача 5.5) ===
var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

lifetime.ApplicationStarted.Register(() =>
{
    Console.WriteLine("\n[App] 🟢 ApplicationStarted: all services initialized");
});

lifetime.ApplicationStopping.Register(() =>
{
    Console.WriteLine("\n[App] 🟡 ApplicationStopping: shutdown signal received");
});

lifetime.ApplicationStopped.Register(() =>
{
    Console.WriteLine("\n[App] 🔴 ApplicationStopped: all services completed");
});
// =========================================================

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

// 4. Через IOptionsSnapshot<T> (per-scope изоляция)
// Console.WriteLine("\n=== Config: IOptionsSnapshot<T> (Per-Scope) ===");
// Console.WriteLine("[Scope 1] Reading config...");
// using (var scope1 = host.Services.CreateScope())
// {
//     var snap1 = scope1.ServiceProvider.GetRequiredService<IOptionsSnapshot<AppSettings>>();
//     Console.WriteLine($"[Scope 1] Timeout: {snap1.Value.Timeout} (frozen for this scope)");
// }

// Console.WriteLine("\n💡 Измени appsettings.json сейчас, если хочешь проверить обновление между скоупами.");
// Console.WriteLine("⏳ Ждём 20 секунд...");
// await Task.Delay(20000);

// using (var scope2 = host.Services.CreateScope())
// {
//     var snap2 = scope2.ServiceProvider.GetRequiredService<IOptionsSnapshot<AppSettings>>();
//     Console.WriteLine($"[Scope 2] Timeout: {snap2.Value.Timeout} (new snapshot)");
// }

// ----------------------------------------------------------------------------
// Запуск хоста + интеграционный тест завершения (Задача 5.4)
// ----------------------------------------------------------------------------

// Замер времени завершения: стартуем таймер в момент получения сигнала остановки
// var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
var shutdownTimer = new System.Diagnostics.Stopwatch();
lifetime.ApplicationStopping.Register(() => shutdownTimer.Start());

// Запускаем хост (блокирует поток до остановки)
await host.RunAsync();

// Хост завершён — останавливаем таймер и выводим результат
shutdownTimer.Stop();
Console.WriteLine($"\n[Test] Graceful shutdown completed in {shutdownTimer.ElapsedMilliseconds} ms");
if (shutdownTimer.ElapsedMilliseconds < 1000)
    Console.WriteLine("[Test] ✅ PASS: Shutdown < 1s");
else
    Console.WriteLine("[Test] ⚠️ SLOW: Shutdown > 1s, check blocking calls");