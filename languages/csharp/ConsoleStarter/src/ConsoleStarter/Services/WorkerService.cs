using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ConsoleStarter.Services;

public class WorkerService : BackgroundService
{
    private readonly IOptionsMonitor<AppSettings> _monitor;
    private readonly string _serviceName;

    public WorkerService(IOptionsMonitor<AppSettings> monitor)
    {
        _monitor = monitor;
        _serviceName = "Worker";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine($"[{_serviceName}] Started");

        while (!stoppingToken.IsCancellationRequested)
        {
            // === Ключевой момент: читаем актуальное значение на каждой итерации ===
            var timeout = _monitor.CurrentValue.Timeout;
            var name = _monitor.CurrentValue.Name;

            Console.WriteLine($"[{_serviceName}] Processing '{name}' with timeout: {timeout}s");

            // Имитация работы
            await Task.Delay(timeout * 1000, stoppingToken);
        }

        Console.WriteLine($"[{_serviceName}] Stopped");
    }
}