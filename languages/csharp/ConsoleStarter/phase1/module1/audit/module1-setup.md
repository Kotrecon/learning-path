# Module 1: Baseline Program.cs Setup

## Структура

- `src/ConsoleStarter/Program.cs`: Top-level statements + `IHost` инициализация
- `src/ConsoleStarter/ConsoleStarter.csproj`: .NET 10, пакет `Microsoft.Extensions.Hosting`

  ```powershell
  dotnet add src/ConsoleStarter package Microsoft.Extensions.Hosting
  ```

- `phase1/module1/audit/`: Артефакты диагностики и аудита

## Архитектурные решения

1. **Top-level statements:** Убран boilerplate `class Program`/`static void Main`. Компилятор Roslyn генерирует скрытый класс и точку входа автоматически.
2. **Host.CreateDefaultBuilder():** Выбран вместо `CreateEmptyBuilder` для получения конфигурационного пайплайна и стандартных логгеров «из коробки» по конвенции .NET.
3. **await host.RunAsync():** Блокирующий вызов, обеспечивающий graceful shutdown, обработку `CancellationToken` и корректное освобождение ресурсов DI.
4. **Компактность (<10 строк):** Снижает когнитивную нагрузку, упрощает код-ревью. JIT-оптимизация сохраняет runtime-производительность.

## Тестирование

- `dotnet run` → старт без исключений, логи `Microsoft.Hosting.Lifetime`
- `Ctrl+C` → graceful shutdown, последовательность логов подтверждена
- `echo $LASTEXITCODE` → `0`
