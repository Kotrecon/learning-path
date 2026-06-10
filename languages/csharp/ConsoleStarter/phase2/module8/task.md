# Модуль 8 — OpenTelemetry Integration

> Цель: Подключить OTel SDK, настроить консольный/OTLP экспорт для Traces, Metrics, Logs.

---

## Секция 1: Инициализация OTel SDK

### Задача 8.1: Настроить базовую инициализацию OpenTelemetry SDK с провайдерами сигналов

**Описание:** Изучить архитектуру OTel SDK: `TracerProvider`, `MeterProvider`, `LoggerProvider`. Подключить необходимые NuGet-пакеты и инициализировать SDK через `AddOpenTelemetry()`. Настроить минимальную конфигурацию для трейсов и метрик.

**Способы достижения:**

- Добавить пакеты: `dotnet add package OpenTelemetry.Exporter.Console OpenTelemetry.Exporter.OpenTelemetryProtocol`
- В `Program.cs` добавить: `builder.Services.AddOpenTelemetry().WithTracing(t => t.AddSource("EP.ConsoleStarter")).WithMetrics(m => m.AddMeter("EP.ConsoleStarter"))`
- Проверить сборку: `dotnet build` без ошибок, убедиться в подключении пакетов
- Закоммитить: `git add .`
  `git commit -m "feat(module8): add OpenTelemetry SDK initialization"`

**Результат:** Блок `AddOpenTelemetry` в Program.cs, успешная сборка, коммит с инициализацией SDK в репозитории.

---

## Секция 2: ActivitySource для трассировки

### Задача 8.2: Реализовать создание трейсов через ActivitySource с тегами

**Описание:** Изучить класс `ActivitySource` как фабрику для создания трейсов. Реализовать статический экземпляр с именем приложения. Научиться начинать спаны через `StartActivity()` и добавлять теги через `SetTag()`.

**Способы достижения:**

- Объявить: `private static readonly ActivitySource Source = new("EP.ConsoleStarter", "1.0.0")`
- В бизнес-логике: `using var activity = Source.StartActivity("ProcessData"); activity?.SetTag("item.count", 10)`
- Добавить `AddConsoleExporter()` в конфигурацию трейсов и увидеть вывод спана в консоли
- Закоммитить: `git commit -am "feat(module8): implement ActivitySource tracing with tags"`

**Результат:** Консольный вывод трейса с тегом `item.count=10`, коммит с реализацией трассировки в репозитории.

---

## Секция 3: Meter для кастомных метрик

### Задача 8.3: Создать инструменты метрик через Meter API

**Описание:** Понять, что `Meter` — фабрика для инструментов метрик: Counter, Histogram, Gauge. Создать статический экземпляр Meter с именем приложения. Реализовать счётчик `items.processed` и инкрементировать его в коде.

**Способы достижения:**

- Объявить: `private static readonly Meter Meter = new("EP.ConsoleStarter", "1.0.0")`
- Создать инструмент: `private static readonly Counter<long> ItemsProcessed = Meter.CreateCounter<long>("items.processed")`
- Инкрементировать: `ItemsProcessed.Add(1, new("item.type", "report"))`
- Добавить `AddConsoleExporter()` для метрик и увидеть пуш в консоли
- Закоммитить: `git commit -am "feat(module8): implement Meter for custom metrics"`

**Результат:** Консольный вывод метрики `items.processed` с атрибутом `item.type=report`, коммит с настройкой метрик.

---

## Секция 4: Интеграция экспортеров Console и OTLP

### Задача 8.4: Настроить вывод сигналов в консоль и через OTLP-протокол

**Описание:** Собрать полную конфигурацию экспортеров для трёх сигналов: Traces, Metrics, Logs. Настроить `AddConsoleExporter` для отладки и `AddOtlpExporter` для продакшена. Убедиться, что данные корректно выводятся в обоих случаях.

**Способы достижения:**

- В конфигурации: `.WithTracing(t => t.AddConsoleExporter().AddOtlpExporter())` и аналогично для Metrics/Logs
- Запустить `dotnet run` и увидеть вывод трейсов, метрик и логов в консоли
- Проверить конфигурацию OTLP: `options.Endpoint = new Uri("http://localhost:4317")`
- Закоммитить: `git add .`
  `git commit -m "feat(module8): configure Console & OTLP exporters"`

**Результат:** Вывод Console Exporter с тремя сигналами, конфигурация OTLP endpoint, коммит с интеграцией экспортеров.

---

## Секция 5: Настройка OTLP-протокола и endpoint

### Задача 8.5: Явно указать протокол и endpoint для OTLP-экспорта

**Описание:** Изучить различия между gRPC и HTTP/Protobuf в OTLP. Понять стандартный endpoint `localhost:4317` для gRPC. Настроить явное указание протокола в конфигурации экспортера для предсказуемости поведения.

**Способы достижения:**

- В конфигурации: `AddOtlpExporter(opt => { opt.Protocol = OtlpExportProtocol.Grpc; opt.Endpoint = new Uri("http://localhost:4317"); })`
- Проверить, что `OtlpExportProtocol` указан явно, не по умолчанию
- При наличии: запустить локальный OTel Collector и проверить приём данных
- Закоммитить: `git commit -am "chore(module8): configure OTLP gRPC endpoint explicitly"`

**Результат:** Конфиг `AddOtlpExporter` с явным `OtlpExportProtocol.Grpc` и endpoint, коммит с настройкой протокола.

---

## Секция 6: Документация и сдача модуля

### Задача 8.6: Подготовить документацию и открыть PR для сдачи модуля

**Описание:** Создать README.md с примерами инициализации OTel SDK, создания трейсов/метрик и настройки экспортеров. Провести финальную проверку: Console exporter выводит все три сигнала, OTLP endpoint настроен. Открыть Pull Request.

**Способы достижения:**

- Создать `module8/README.md` с примерами кода и инструкцией по проверке вывода
- Прогнать финальный тест: `dotnet run` → увидеть трейсы, метрики, логи в консоли
- Создать ветку `feature/module8-complete`, запушить и открыть PR с чек-листом
- Приложить к PR скриншот вывода Console Exporter

**Результат:** Документированный модуль в README.md, открыт PR с меткой "Модуль 8 завершён", все критерии подтверждены.

✅ **Критерий приёмки:**

- OTel SDK подключен через `AddOpenTelemetry`
- `ActivitySource` создаёт трейсы с тегами
- `Meter` создаёт и инкрементирует счётчики
- Console Exporter выводит Traces, Metrics и Logs в консоль
- OTLP Exporter настроен на стандартный endpoint (`localhost:4317`)
- Все изменения зафиксированы в Git с понятной историей коммитов

---
