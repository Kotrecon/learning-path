using Microsoft.Extensions.Options;

namespace ConsoleStarter.Services;

public class ConfigMonitorService
{
    private readonly IOptionsMonitor<AppSettings> _monitor;

    public ConfigMonitorService(IOptionsMonitor<AppSettings> monitor)
    {
        _monitor = monitor;
    }

    public void PrintCurrentConfig()
    {
        var current = _monitor.CurrentValue;
        Console.WriteLine($"\n[Monitor] Name: {current.Name}, Timeout: {current.Timeout}");
    }
}