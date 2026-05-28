using Microsoft.Extensions.Hosting;

namespace ConsoleStarter.Services;

public class PipelineWorker : BackgroundService
{
    private readonly string _serviceName;
    private bool _isResourceActive;

    public PipelineWorker()
    {
        _serviceName = "PipelineWorker";
        _isResourceActive = false;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine($"[{_serviceName}] Started");

        // 1. Инициализация ресурса (БД, файл, очередь и т.д.)
        await OpenResourceAsync();

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine($"[{_serviceName}] Processing with active resource...");
                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"[{_serviceName}] Cancellation requested. Finishing current iteration...");
        }
        finally
        {
            // 2. Гарантированная очистка (выполнится ВСЕГДА)
            await CloseResourceAsync();
        }

        Console.WriteLine($"[{_serviceName}] Stopped gracefully");
    }

    private Task OpenResourceAsync()
    {
        _isResourceActive = true;
        Console.WriteLine($"[{_serviceName}] 📂 Resource opened (DB connection / file handle)");
        return Task.CompletedTask;
    }

    private Task CloseResourceAsync()
    {
        if (_isResourceActive)
        {
            _isResourceActive = false;
            Console.WriteLine($"[{_serviceName}] 📂 Resource closed & buffers flushed");
        }
        return Task.CompletedTask;
    }
}