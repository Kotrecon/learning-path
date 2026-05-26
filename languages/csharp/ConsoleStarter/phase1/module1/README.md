# Модуль 1 — .NET 10 Console Modernization

## Описание

Базовая настройка консольного приложения на .NET 10 с использованием top-level statements и IHost.

```powershell
dotnet add src/ConsoleStarter package Microsoft.Extensions.Hosting
```

```csharp
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args).Build();

await host.RunAsync();

```

## Запуск и проверка

1. Сборка: `dotnet build`
2. Запуск: `dotnet run`
3. Остановка: Нажмите `Ctrl+C` для graceful shutdown.
4. Проверка кода выхода: `echo $LASTEXITCODE` (должно быть `0`).

## Критерии приёмки

- [x] Top-level statements (нет class Program)
- [x] IHost инициализирован через CreateDefaultBuilder
- [x] Graceful shutdown подтверждён

> - `[Console]::OutputEncoding = [System.Text.Encoding]::UTF8` - исправляет кодировку.
> - `Воспроизводимость`: `CI/CD` или новый разработчик сразу увидит, на какой версии `SDK` и с какими пакетами проект гарантированно собирался - `dotnet --info`.
> - `Аудит графа зависимостей`: `dotnet list package` покажет прямые и транзитивные зависимости, что критично при переходе к `AOT/trimming` или проверке лицензий.
> - `Отладка сборки`: `dotnet build -v:d` даёт полный лог `MSBuild`. При ошибках компиляции или проблемах с линковкой это первый файл, который запрашивают в поддержке `.NET`.
