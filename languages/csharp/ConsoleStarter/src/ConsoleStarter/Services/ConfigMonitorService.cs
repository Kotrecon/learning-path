using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ConsoleStarter.Services;

public class ConfigMonitorService : IDisposable
{
    private readonly IOptionsMonitor<AppSettings> _monitor;
    private readonly IDisposable? _subscription;
    private readonly string _serviceName;
    private readonly ILogger<ConfigMonitorService> _logger;

    public ConfigMonitorService(IOptionsMonitor<AppSettings> monitor, ILogger<ConfigMonitorService> logger)
    {
        _monitor = monitor;
        _serviceName = "ConfigMonitor";
        _logger = logger;

        // Простая подписка на изменения конфига
        _subscription = _monitor.OnChange((settings, name) =>
        {
            Console.WriteLine("[{_serviceName}] Config changed!", _serviceName);

            _logger.LogInformation("[{_serviceName}] Name={Name}, Timeout={Timeout}", _serviceName, settings.Name, settings.Timeout);
        });
    }

    public void PrintCurrentConfig()
    {
        var current = _monitor.CurrentValue;
        _logger.LogInformation("[{_serviceName}] Name={Name}, Timeout={Timeout}", _serviceName, current.Name, current.Timeout);
    }

    // Хорошая практика: отписываемся при уничтожении сервиса
    public void Dispose()
    {
        _subscription?.Dispose();
    }
}