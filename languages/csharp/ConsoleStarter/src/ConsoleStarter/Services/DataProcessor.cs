namespace ConsoleStarter.Services;

public class DataProcessor : IDataProcessor, IDisposable
{
    private readonly string _instanceId = Guid.NewGuid().ToString("N")[..8];
    private bool _disposed;
    public DataProcessor()
    {
        Console.WriteLine($"[DataProcessor] Создан: {_instanceId}");

    }

    public void Process(string data)
    {
        Console.WriteLine($"[DataProcessor] Работа: {_instanceId} | {data}");
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            Console.WriteLine($"[DataProcessor] Освобождён: {_instanceId}");
            _disposed = true;
        }
    }
}