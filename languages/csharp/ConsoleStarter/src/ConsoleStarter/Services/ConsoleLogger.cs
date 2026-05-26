namespace ConsoleStarter.Services;

public class ConsoleLogger : ILoggerService, IDisposable
{
    private readonly string _instanceId = Guid.NewGuid().ToString("N")[..8];
    private bool _disposed;

    public ConsoleLogger()
    {
        Console.WriteLine($"[ConsoleLogger] Создан: {_instanceId}");
    }

    public void Log(string message)
    {
        Console.WriteLine($"[ConsoleLogger] Создан: {_instanceId} | {message}");
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            Console.WriteLine($"[ConsoleLogger] Освобождён: {_instanceId}");
            _disposed = true;
        }
    }
}