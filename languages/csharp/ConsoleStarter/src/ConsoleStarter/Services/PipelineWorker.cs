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
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine($"[{_serviceName}] Processing...");
                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Ожидаемое завершение: токен отменён, выходим штатно
            Console.WriteLine($"[{_serviceName}] Cancellation requested");
        }
        finally
        {
            // Выполнится ВСЕГДА: и при штатном выходе из цикла, и при эксепшене
            Console.WriteLine($"[{_serviceName}] Stopped & resources released");
        }
    }
}