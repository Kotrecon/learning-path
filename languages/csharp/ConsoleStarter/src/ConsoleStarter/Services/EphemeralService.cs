namespace ConsoleStarter.Services;

public class EphemeralService : ITransientService, IDisposable
{
    private readonly string _instanceId = Guid.NewGuid().ToString("N")[..8];
    private bool _disposed;

    public EphemeralService()
    {
        Console.WriteLine($"[EphemeralService] Создан: {_instanceId}");
    }

    public void DoWork(string operation)
    {
        Console.WriteLine($"[EphemeralService] Работа: {_instanceId} | {operation}");
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            Console.WriteLine($"[EphemeralService] Освобождён: {_instanceId}");
            _disposed = true;
        }
    }
}