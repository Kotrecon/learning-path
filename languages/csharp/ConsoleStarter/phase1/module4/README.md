# Модуль 4 — 🔁 Options & Runtime Reconfiguration

> Цель: Подписаться на изменения конфига, применять изменения без рестарта, валидировать на старте и в рантайме.

---

## 🔧 Требования к пакетам

Для работы валидации через `ValidateDataAnnotations()` добавьте пакет:

```powershell
dotnet add package Microsoft.Extensions.Options.DataAnnotations
```

Или вручную в `.csproj`:

```xml
<PackageReference Include="Microsoft.Extensions.Options.DataAnnotations" Version="10.0.*" />
```

---

## 🔑 Ключевые концепции: Явное vs Неявное

| Действие                   | Работает неявно (фреймворк)                                     | Требует явного кода                                                         |
| -------------------------- | --------------------------------------------------------------- | --------------------------------------------------------------------------- |
| **Реактивное чтение**      | `IOptionsMonitor<T>` подписывается на `IChangeToken` провайдера | `builder.Services.AddSingleton<T>()` + резолв `.CurrentValue`               |
| **Hot-reload файла**       | `FileSystemWatcher` отслеживает изменения                       | `builder.Configuration.AddJsonFile(..., reloadOnChange: true)`              |
| **Валидация при старте**   | Фабрика опций создаётся при старте хоста                        | `.ValidateDataAnnotations().ValidateOnStart()` + пакет `DataAnnotations`    |
| **Валидация в рантайме**   | При изменении файла фабрика пересоздаёт объект                  | `[Required]`, `[Range]` на свойствах record                                 |
| **Per-Scope стабильность** | `IOptionsSnapshot<T>` регистрируется автоматически              | Резолв внутри `using var scope = host.Services.CreateScope()`               |
| **Graceful Shutdown**      | Хост отменяет `CancellationToken` при `Ctrl+C` / `SIGTERM`      | Цикл `while (!stoppingToken.IsCancellationRequested)` в `BackgroundService` |

---

## 🛠️ Быстрый старт

### 1. Реактивный доступ (`IOptionsMonitor<T>`)

```csharp
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Services.AddOptions<AppSettings>().BindConfiguration("App");

var monitor = host.Services.GetRequiredService<IOptionsMonitor<AppSettings>>();
Console.WriteLine(monitor.CurrentValue.Timeout); // Всегда актуальное значение
```

### 2. Подписка на изменения (`OnChange`)

```csharp
monitor.OnChange((settings, name) =>
{
    Console.WriteLine($"Config changed: Timeout={settings.Timeout}");
    // Идемпотентность: не делай тяжёлую логику, debounce при необходимости
});
```

### 3. Валидация (Fail-Fast)

```csharp
// AppSettings.cs
[Required] public string Name { get; init; }
[Range(1, 3600)] public int Timeout { get; init; }

// Program.cs
builder.Services.AddOptions<AppSettings>()
    .BindConfiguration("App")
    .ValidateDataAnnotations()
    .ValidateOnStart(); // Приложение не стартует с invalid-конфигом
```

### 4. Фоновый сервис с hot-reload

```csharp
builder.Services.AddHostedService<WorkerService>();

// В WorkerService.ExecuteAsync(CancellationToken ct)
while (!ct.IsCancellationRequested)
{
    var timeout = _monitor.CurrentValue.Timeout;
    // Логика...
    await Task.Delay(timeout * 1000, ct);
}
```

### 5. Per-Scope снимок (`IOptionsSnapshot<T>`)

```csharp
using var scope = host.Services.CreateScope();
var snapshot = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<AppSettings>>();
// Значение заморожено на время жизни scope. Изменения файла не влияют внутри scope.
```

---

## 🧪 Ручное тестирование hot-reload

1. Запусти `dotnet run`.
2. Открой `appsettings.json` в редакторе.
3. **~20 секунд**: поменяй `"Timeout": 30` → `"Timeout": 60`, сохрани.
4. В консоли увидь:
   - `🔄 [ConfigMonitor] Config changed!`
   - `[Worker] Processing '...' with timeout: 60s` (применилось без рестарта)
5. Проверь валидацию: удали `"Name"` → приложение упадёт с `OptionsValidationException` при старте или при сохранении файла (runtime fail-fast).

---

## ✅ Критерии приёмки

- [x] `IOptionsMonitor<AppSettings>` инжектится и предоставляет `CurrentValue`
- [x] `OnChange` callback вызывается при изменении `appsettings.json` без перезапуска
- [x] Валидация через `ValidateDataAnnotations` + `ValidateOnStart` блокирует запуск при ошибке
- [x] `BackgroundService` применяет новое значение конфига в рантайме
- [x] `IOptionsSnapshot<T>` демонстрирует per-scope изоляцию
- [x] Документация чётко разделяет явные и неявные действия, содержит примеры кода
- [x] Все изменения зафиксированы в Git с понятной историей коммитов

> 💡 **Production-инсайт**: `OnChange` может срабатывать несколько раз на одно сохранение файла (особенность `FileSystemWatcher`). Делай коллбэк идемпотентным или используй debounce для тяжёлых операций.

---

### 💾 Фиксация (Git)

```powershell
git add src/ConsoleStarter/Program.cs phase1/module4/README.md
git commit -m "chore(module4): clean Program.cs, add package note to README"
git push origin feature/csharp/console-starter/phase1/module4
```

---
