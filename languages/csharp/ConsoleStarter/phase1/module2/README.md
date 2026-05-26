# Модуль 2 — DI & Lifetime Management

## 🎯 Цель

Научиться управлять временем жизни зависимостей в консольном приложении .NET 10, предотвращать утечки ресурсов и верифицировать поведение контейнера.

## 🔑 Ключевые паттерны

```csharp
// 1. Регистрация (HostApplicationBuilder)
builder.Services.AddSingleton<ILoggerService, ConsoleLogger>();
builder.Services.AddScoped<IDataProcessor, DataProcessor>();
builder.Services.AddTransient<ITransientService, EphemeralService>();

// 2. Ручной скоуп в консоли (обязательно для Scoped!)
var factory = host.Services.GetRequiredService<IServiceScopeFactory>();
using var scope = factory.CreateScope();
var svc = scope.ServiceProvider.GetRequiredService<IDataProcessor>();
// Dispose вызывается автоматически при выходе из using
```
