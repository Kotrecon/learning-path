using Microsoft.Extensions.Options;

namespace ConsoleStarter.Services;

public class ConfigMonitorService : IDisposable
{
    private readonly IOptionsMonitor<AppSettings> _monitor;
    private readonly IDisposable? _subscription;
    private readonly string _serviceName;

    public ConfigMonitorService(IOptionsMonitor<AppSettings> monitor)
    {
        _monitor = monitor;
        _serviceName = "ConfigMonitor";

        // Простая подписка на изменения конфига
        _subscription = _monitor.OnChange((settings, name) =>
        {
            Console.WriteLine($"\n🔄 [{_serviceName}] Config changed!");
            Console.WriteLine($"   New values: Name={settings.Name}, Timeout={settings.Timeout}");
        });
    }

    public void PrintCurrentConfig()
    {
        var current = _monitor.CurrentValue;
        Console.WriteLine($"[{_serviceName}] Current: Name={current.Name}, Timeout={current.Timeout}");
    }

    // Хорошая практика: отписываемся при уничтожении сервиса
    public void Dispose()
    {
        _subscription?.Dispose();
    }
}