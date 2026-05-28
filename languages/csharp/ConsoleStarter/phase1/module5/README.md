# Модуль 5 — 🔄 BackgroundService & Cancellation

> Цель: Реализовать фоновый воркер с корректной отменой через CancellationToken и гарантированной очисткой ресурсов.

---

## 🔑 Ключевые концепции: Явное vs Неявное

| Действие                     | Работает неявно (фреймворк)                                   | Требует явного кода                                                     |
| ---------------------------- | ------------------------------------------------------------- | ----------------------------------------------------------------------- |
| **Запуск воркера**           | `BackgroundService` интегрируется в `IHostedService`          | `builder.Services.AddHostedService<PipelineWorker>()`                   |
| **Отмена по сигналу**        | Хост отменяет `CancellationToken` при `Ctrl+C` / `SIGTERM`    | Проверка `while (!stoppingToken.IsCancellationRequested)` в цикле       |
| **Мгновенная реакция**       | `Task.Delay(..., token)` бросает `OperationCanceledException` | Передача токена во все async-операции                                   |
| **Гарантированная очистка**  | `finally` выполняется всегда при выходе из метода             | Обёртывание цикла в `try/finally` с закрытием ресурсов                  |
| **События жизненного цикла** | `IHostApplicationLifetime` регистрируется автоматически       | `lifetime.ApplicationStopping.Register(...)` для инфраструктурных хуков |

---

## 🛠️ Быстрый старт

### 1. Базовый воркер

```csharp
public class PipelineWorker : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Полезная работа...
            await Task.Delay(1000, stoppingToken); // ← токен обязателен!
        }
    }
}
// Регистрация:
builder.Services.AddHostedService<PipelineWorker>();
```

### 2. Graceful shutdown с очисткой

```csharp
try
{
    while (!stoppingToken.IsCancellationRequested)
    {
        await DoWorkAsync(stoppingToken);
    }
}
catch (OperationCanceledException)
{
    // Штатная отмена — не ошибка
}
finally
{
    await CleanupResourcesAsync(); // ← Выполнится ВСЕГДА
}
```

### 3. Замер времени завершения (интеграционный тест)

```csharp
var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
var timer = new Stopwatch();
lifetime.ApplicationStopping.Register(() => timer.Start());

await host.RunAsync();

timer.Stop();
Console.WriteLine($"Shutdown: {timer.ElapsedMilliseconds} ms");
```

### 4. События жизненного цикла

```csharp
lifetime.ApplicationStarted.Register(() => Console.WriteLine("🟢 Started"));
lifetime.ApplicationStopping.Register(() => Console.WriteLine("🟡 Stopping"));
lifetime.ApplicationStopped.Register(() => Console.WriteLine("🔴 Stopped"));
```

---

## 🧪 Ручное тестирование

1. Запусти `dotnet run`.
2. Увидь: `[App] 🟢 ApplicationStarted`, `[PipelineWorker] Started`.
3. Дай поработать ~3 секунды.
4. Нажми `Ctrl+C`.
5. **Ожидаемая последовательность:**

   ```bash
   [App] 🟡 ApplicationStopping: shutdown signal received
   [PipelineWorker] Cancellation requested...
   [PipelineWorker] 📂 Resource closed & buffers flushed
   [PipelineWorker] Stopped gracefully
   [App] 🔴 ApplicationStopped: all services completed
   [Test] Graceful shutdown completed in XX ms
   [Test] ✅ PASS: Shutdown < 1s
   ```

---

## ✅ Критерии приёмки

- [x] `PipelineWorker` наследуется от `BackgroundService` и регистрируется через `AddHostedService`
- [x] `ExecuteAsync` содержит цикл с проверкой `stoppingToken.IsCancellationRequested`
- [x] `Task.Delay(..., stoppingToken)` используется для кооперативной отмены
- [x] `try/catch/finally` гарантирует очистку ресурсов при любой ситуации
- [x] При нажатии `Ctrl+C` воркер завершается за <1 секунды
- [x] `IHostApplicationLifetime` регистрирует хуки для инфраструктурного логирования
- [x] Документация содержит примеры кода и пошаговую инструкцию проверки

> 💡 **Production-инсайт**: `finally` — это не опция, а контракт. Если ресурс открыт в `try`, он обязан закрыться в `finally`. Никаких `if`, никаких исключений.

---
