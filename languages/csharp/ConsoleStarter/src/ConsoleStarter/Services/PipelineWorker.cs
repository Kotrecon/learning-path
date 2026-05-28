using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsoleStarter.Services;

public class PipelineWorker : BackgroundService
{
    private readonly ILogger<PipelineWorker> _logger;

    public PipelineWorker(ILogger<PipelineWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new InvalidOperationException("🔥 TEST: Simulated startup failure");

        _logger.LogInformation("Worker started");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug("Processing iteration...");

                // Симуляция работы: иногда логируем Warning для демонстрации уровней
                if (DateTime.Now.Second % 10 == 0)
                {
                    _logger.LogWarning("Retry attempt detected (demo)");
                }

                _logger.LogInformation("Processing with active resource...");
                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Cancellation requested. Finishing current iteration...");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in worker loop");
            throw; // Пробрасываем дальше, чтобы сработал глобальный обработчик в Main
        }
        finally
        {
            _logger.LogInformation("Resource closed & buffers flushed");
        }

        _logger.LogInformation("Worker stopped gracefully");
    }
}