using Microsoft.Extensions.Hosting;

namespace ConsoleStarter.Services;

public class PipelineWorker : BackgroundService
{
    private readonly string _serviceName;

    public PipelineWorker()
    {
        _serviceName = "PipelineWorker";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine($"[{_serviceName}] Started");

        try
        {
            // 1. Кооперативная проверка: выходим только когда токен отменён
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine($"[{_serviceName}] Processing...");

                // 2. Токен передаётся в Delay → мгновенная реакция на Ctrl+C
                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            // 3. Штатная отмена: не ошибка, а плановое завершение
            Console.WriteLine($"[{_serviceName}] Cancellation requested. Exiting loop...");
        }
        finally
        {
            // 4. Гарантированная очистка (выполнится ВСЕГДА)
            Console.WriteLine($"[{_serviceName}] Stopped & resources released");
        }
    }
}