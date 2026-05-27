# 🖥️ `.NET 10 Console Engineering`. Полная дорожная карта обучения (52 модуля)

---

> 💡 `Современное консольное приложение на .NET 10` — это `инженерный процесс-оркестратор с детерминированным жизненным циклом, фоновым конвейером, сквозной наблюдаемостью и безопасным управлением данными`, а не «скрипт с `Console.WriteLine()`». Production-реализация строится на `Microsoft.Extensions.Hosting`, `BackgroundService` + `System.Threading.Channels`, `OpenTelemetry`, `структурированном аудите`, `DPAPI/ENV`, `базовом EF Core`, `Polly-резильентности`, `TUnit` и `dotnet new шаблонизации`. Ключ к мастерству: `архитектура = DI-first`, `поток = асинхронный конвейер`, `наблюдаемость = сквозная`, `ошибки = стратегически разделены`, `данные = базовы`, `доставка = CI-гейтед`.

## 📋 Оглавление

- [Основная мысль](#-основная-мысль)
- [Проблема](#️-проблема)
- [Ключевые термины](#-ключевые-термины)
- [Структура обучения: 8 фаз (52 модуля)](#️-структура-обучения-8-фаз-52-модуля)
- [Что получим по окончании курса](#-что-получим-по-окончании-курса)
- [Практические примеры кода (C# .NET 10)](#-практические-примеры-кода-c-net-10)
- [Production-чек-лист](#-production-чек-лист)
- [Заключение + план действий](#-заключение--план-действий)

---

## 💡 Основная мысль

`Изучение production .NET 10 Console Engineering` решает проблему `«хаотичных скриптов без логов, memory leak в фоновых циклах, отсутствия аудита, небезопасных конфигов, невозможности отладки в prod и размытой границы между бизнес-ошибками и системными крашами»`: вместо `static void Main()` ты получаешь `верифицируемый конвейер` с `BackgroundService`, `Channel<T>` backpressure, `OpenTelemetry`-экспортом, неизменяемым аудит-трейлом, `IOptionsMonitor` hot-reload, `Result<T>` для бизнес-логики, глобальным хэндлером для системных ошибок и `dotnet new`-упаковкой. Архитектура обучения строится на `пяти принципах`: (1) `Host-First & Pipeline-Driven` (`IHost`, `BackgroundService`, `Channel<T>`, scope-per-iteration), (2) `Observability-by-Default` (OTel traces/metrics/logs, correlation IDs, structured output), (3) `Zero-Trust Security & Audit` (secrets isolation, input validation, immutable audit, DPAPI/ENV), (4) `Result-Driven & Resilient` (business vs system errors, Polly, basic EF Core), (5) `Template-Ready & CI-Gated` (`dotnet new`, TUnit analyzers, security scan, automated pack). Компромисс: `требует дисциплины в управлении скоупами, observability и error-паттернах` окупается `×5 стабильностью`, `нулевым memory leak`, `аудит-готовностью` и `переиспользуемостью шаблона`.

---

## ⚠️ Проблема

| Сценарий                                  | Без engineering-подхода                                 | С .NET 10 Console Engineering                                    |
| ----------------------------------------- | ------------------------------------------------------- | ---------------------------------------------------------------- |
| `static void Main()` + цикл `while(true)` | Memory leak, нет graceful shutdown, сложно тестировать  | `BackgroundService`, `IServiceScopeFactory`, `CancellationToken` |
| Конфиги в коде                            | Секреты в репо, нет ротации, уязвимости                 | `IOptionsMonitor<T>`, env precedence, hot-reload без рестарта    |
| Отсутствие аудита                         | Кто что изменил? Когда? Почему? → неизвестно            | Append-only log, hash-chained entries, tamper detection          |
| `try/catch` повсюду                       | Business errors смешаны с system crashes, код раздут    | `Result<T>` для бизнес-логики, Global Handler для системных      |
| Прямые `new HttpClient()` / БД            | Нет retry, circuit breaker, resource leak               | `IHttpClientFactory`, Polly, scoped `DbContext`, resilience      |
| Нет конвейера данных                      | Producer/Consumer блокируются, backpressure отсутствует | `Channel<T>`, bounded queue, async enumeration                   |
| Нет шаблона/тестов                        | Копипаст, дрифт стандартов, xUnit/Moq оверхед           | `dotnet new`, TUnit async-first, CI gates                        |

**Без системного подхода**: Консольное приложение превращается в «фоновый зомби», который жрёт память, молча падает, не проходит аудит и не поддаётся контролю в production.

---

## 📚 Ключевые термины

| Термин                                  | Определение                                                              | Роль                                                                    |
| --------------------------------------- | ------------------------------------------------------------------------ | ----------------------------------------------------------------------- |
| `BackgroundService & CancellationToken` | `StartAsync` → цикл обработки → `StopAsync`, cooperative cancellation    | `Lifecycle`: стандартный паттерн long-running console workers           |
| `IServiceScopeFactory`                  | Создание `Scoped` сервисов внутри цикла без `static` и leak              | `Memory`: безопасное управление `DbContext`, HTTP-клиентами             |
| `Channel<T>`                            | Producer/Consumer pipeline, bounded queue, backpressure                  | `Throughput`: неблокирующий конвейер, контроль нагрузки                 |
| `IOptionsMonitor<T>`                    | Runtime hot-reload конфигурации, `OnChange`, validation                  | `Flexibility`: переконфигурация без перезапуска процесса                |
| `OpenTelemetry (OTel)`                  | Traces, Metrics, Logs export, `ActivitySource`, `Meter`                  | `Observability`: сквозная наблюдаемость, совместимость с Grafana/Jaeger |
| `Result<T> Pattern`                     | Явное возвращение успеха/ошибки вместо `try/catch`                       | `Predictability`: бизнес-ошибки = данные, системные = исключения        |
| `Global Exception Handler`              | `AppDomain.UnhandledException` + `TaskScheduler.UnobservedTaskException` | `Reliability`: централизованный catch, OTel flush, exit-code mapping    |
| `Immutable Audit`                       | Append-only log, hash-chained entries, tamper detection                  | `Compliance`: юридическая доказуемость, GDPR/152-ФЗ mapping             |
| `Basic EF Core`                         | `DbContext`, programmatic migrations, async CRUD, scoped lifetime        | `Persistence`: минимальная персистентность без веб-зависимостей         |
| `TUnit`                                 | Async-first, source-gen, zero-allocation testing framework               | `Quality`: современные тесты без xUnit/Moq оверхеда                     |

---

## 🗂️ Структура обучения: 8 фаз (52 модуля)

### 📘 Фаза 1: Фундамент .NET 10 и архитектура (Модули 1-6)

| №   | Модуль                                 | Эволюция                                                                              | Ключевые навыки                                           | Практика (C# .NET 10)                                                              |
| --- | -------------------------------------- | ------------------------------------------------------------------------------------- | --------------------------------------------------------- | ---------------------------------------------------------------------------------- |
| 1   | `.NET 10 Console Modernization`        | `static Main` → top-level, `Host.CreateApplicationBuilder(args)`, minimal boilerplate | `host routing`, `lifecycle sync`, `startup logic`         | Создать `Program.cs` с `Host.CreateApplicationBuilder(args)`, top-level statements |
| 2   | `DI & Lifetime Management`             | `new` → `AddSingleton/Transient/Scoped`, explicit scopes via `IServiceScopeFactory`   | `di routing`, `lifetime sync`, `dispose logic`            | Зарегистрировать сервисы, проверить `IDisposable`, создать scope вручную в цикле   |
| 3   | `Configuration Pipeline`               | `.config` → `appsettings.json`, env vars (`__` separator), CLI args                   | `config routing`, `precedence sync`, `parse logic`        | Настроить приоритеты: CLI > ENV (`__`) > JSON, автоматизировать проверку           |
| 4   | 🔁 `Options & Runtime Reconfiguration` | `IOptions<T>` → `IOptionsMonitor<T>`, `OnChange`, hot-reload, `ValidateOnStart`       | `options routing`, `reload sync`, `validate logic`        | Подписаться на `OnChange`, применить hot-reload, добавить `ValidateOnStart()`      |
| 5   | 🔄 `BackgroundService & Cancellation`  | `while(true)` → `ExecuteAsync`, cooperative `CancellationToken`                       | `worker routing`, `cancel sync`, `lifecycle logic`        | Реализовать воркер с корректной отменой, написать TUnit-тест завершения            |
| 6   | `Baseline Console Pipeline`            | Init → config → DI → `CreateApplicationBuilder` → run → cancellation → log → publish  | `orchestration design`, `state propagation`, `sync logic` | Собрать baseline, провести parity-тест на TUnit, подготовить `dotnet publish`      |

### 🔍 Фаза 2: Логирование и Observability (OTel) (Модули 7-13)

| №   | Модуль                              | Цель                                                      | Стек | Ключевые навыки                                           | Практика (C# .NET 10)                                       |
| --- | ----------------------------------- | --------------------------------------------------------- | ---- | --------------------------------------------------------- | ----------------------------------------------------------- |
| 7   | `Structured Logging Basics`         | `ILogger<T>`, levels, scopes, message templates           | C#   | `log routing`, `scope sync`, `template logic`             | Настроить `Microsoft.Extensions.Logging` с JSON-форматтером |
| 8   | `OpenTelemetry Integration`         | Traces, Metrics, Logs SDK, `ActivitySource`, `Meter`      | C#   | `otel routing`, `sdk sync`, `export logic`                | Подключить OTel SDK, настроить консольный/OTLP экспорт      |
| 9   | `Correlation & Context Propagation` | `TraceId`, `SpanId`, `Baggage`, `Activity.Current`        | C#   | `context routing`, `propagation sync`, `trace logic`      | Сквозной `CorrelationId` в логах и трейсах                  |
| 10  | `Custom Metrics & Counters`         | Histograms, Counters, Gauges, `Instrument`                | C#   | `metric routing`, `instrument sync`, `hist logic`         | Замерить длительность операций, кол-во ошибок               |
| 11  | `Log Enrichment & Filtering`        | Machine name, user, version, `LogContext`, `MinimumLevel` | C#   | `enrich routing`, `filter sync`, `level logic`            | Добавить enrichers, настроить фильтрацию по тегам           |
| 12  | `OTel Exporters & Batching`         | Console, File, OTLP, batching, retry on flush             | C#   | `export routing`, `batch sync`, `otlp logic`              | Настроить OTLP-экспорт, проверить буферизацию               |
| 13  | `Observability Pipeline`            | Log → enrich → OTel → export → validate                   | C#   | `orchestration design`, `state propagation`, `sync logic` | Собрать pipeline, parity-тест трейсов/метрик                |

### ⚡ Фаза 3: Безопасность и Аудит (Модули 14-21)

| №   | Модуль                                | Цель                                          | Стек | Ключевые навыки                                           | Практика (C# .NET 10)                         |
| --- | ------------------------------------- | --------------------------------------------- | ---- | --------------------------------------------------------- | --------------------------------------------- |
| 14  | `Secure Configuration Secrets`        | DPAPI, ENV, no `.git`, secret rotation        | C#   | `sec routing`, `secret sync`, `rotate logic`              | Настроить безопасную загрузку через ENV/DPAPI |
| 15  | `Input Validation & Sanitization`     | `DataAnnotations`, trim, whitelist            | C#   | `valid routing`, `sanitize sync`, `whitelist logic`       | Валидировать входные данные CLI/ENV           |
| 16  | `Immutable Audit Trail Design`        | Append-only log, `AuditEntry`, hash chaining  | C#   | `audit routing`, `chain sync`, `immut logic`              | Создать неизменяемый аудит-лог                |
| 17  | `Tamper-Evident Logging`              | SHA256 chaining, `PreviousHash`, verification | C#   | `tamper routing`, `hash sync`, `verify logic`             | Реализовать верификацию целостности           |
| 18  | `Security Defaults & Least Privilege` | Secure file access, network restrictions      | C#   | `sec routing`, `privilege sync`, `restrict logic`         | Настроить безопасные умолчания для FS/Network |
| 19  | `Policy-Based Access Control`         | Role simulation, deny-by-default              | C#   | `policy routing`, `role sync`, `deny logic`               | Реализовать базовый RBAC для операций         |
| 20  | `Compliance & PII Masking`            | GDPR/152-ФЗ, data retention, regex masking    | C#   | `compliance routing`, `mask sync`, `export logic`         | Настроить маскирование PII перед записью      |
| 21  | `Security & Audit Pipeline`           | Validate → secure → audit → verify → log      | C#   | `orchestration design`, `state propagation`, `sync logic` | Собрать pipeline, parity-тест целостности     |

### 🌐 Фаза 4: Базовый EF Core и слой данных (Модули 22-28)

| №   | Модуль                                        | Цель                                                           | Стек | Ключевые навыки                                   | Практика (C# .NET 10)                                                                                      |
| --- | --------------------------------------------- | -------------------------------------------------------------- | ---- | ------------------------------------------------- | ---------------------------------------------------------------------------------------------------------- |
| 22  | 🛠️ `EF Core Basics & Programmatic Migrations` | `DbContext`, `MigrateAsync()`, startup retry                   | C#   | `ef routing`, `migrate sync`, `provider logic`    | Автоприменение миграций на старте, откат при ошибке                                                        |
| 23  | 🔁 `Scoped DI in Long-Running Loops`          | `IServiceScopeFactory`, `using scope`, per-iteration           | C#   | `scope routing`, `loop sync`, `dispose logic`     | Паттерн: `while(!ct.IsCancellationRequested) { using(scope) { ... } }`                                     |
| 24  | `Audited Entities`                            | `CreatedBy`, `CreatedAt`, `SaveChanges` override               | C#   | `entity routing`, `audit sync`, `soft logic`      | Добавить аудит-поля в сущности                                                                             |
| 25  | `Basic CRUD & Query Patterns`                 | LINQ, pagination, projection, `LogTo()`                        | C#   | `crud routing`, `query sync`, `proj logic`        | Реализовать операции без N+1                                                                               |
| 26  | `Connection Resilience`                       | Retry, timeout, circuit breaker for DB                         | C#   | `resilience routing`, `retry sync`, `cb logic`    | Интегрировать Polly для EF Core                                                                            |
| 27  | 📦 `Channel<T> Data Pipeline`                 | Bounded queue, backpressure, async enumeration                 | C#   | `channel routing`, `bp sync`, `prod/consum logic` | Конвейер: чтение → валидация → запись в БД                                                                 |
| 28  | Cache-Aside & Full Pipeline Assembly          | `ICacheService<T>`, TTL, invalidation, fallback, orchestration | C#   | `cache routing`, `ttl sync`, `pipeline logic`     | **Собрать полный пайплайн:** `Channel` → `Cache` (hit/miss) → `EF Core` (fallback) → `Audit` → `Result<T>` |

### 🔍 Фаза 5: CLI, Тестирование (TUnit) и Стратегия Ошибок (Модули 29-35)

| №   | Модуль                                                   | Цель                                                               | Ключевые навыки                                   | Практика (C# .NET 10)                                   |
| --- | -------------------------------------------------------- | ------------------------------------------------------------------ | ------------------------------------------------- | ------------------------------------------------------- |
| 29  | 🧪 `Unit Testing с TUnit`                                | `async Task`, `Assert.That()`, DI mocking, zero-alloc              | `test routing`, `async sync`, `di logic`          | Написать тесты через TUnit, мокать `IOptionsMonitor`    |
| 30  | `Integration Testing`                                    | `TestHost`, in-memory DB, CLI arg simulation                       | `integration routing`, `host sync`, `cli logic`   | Протестировать полный цикл с БД и CLI                   |
| 31  | `CLI Argument Parsing`                                   | `System.CommandLine`, subcommands, validation                      | `cli routing`, `parse sync`, `help logic`         | Реализовать CLI с `--json`, `--verbose`, `--dry-run`    |
| 32  | `Structured Output Modes`                                | Human, JSON, plain, `IOutputFormatter`                             | `output routing`, `format sync`, `mode logic`     | Переключение формата вывода без дублирования            |
| 33  | `Error Handling & Exit Codes`                            | `try/catch` mapping, typed exceptions, codes                       | `error routing`, `code sync`, `hint logic`        | Маппинг ошибок в строгие exit-коды                      |
| 34  | 🛡️ `Result Pattern, Exception Strategy & Global Handler` | `Result<T>`, business vs system, `AppDomain`/`TaskScheduler` hooks | `result routing`, `strategy sync`, `global logic` | Разделить ошибки, настроить централизованный обработчик |
| 35  | `Testing & Error Pipeline`                               | Unit → integration → cli → error mapping → lint                    | `test routing`, `sync logic`, `gate logic`        | Собрать testing pipeline, zero-regression               |

### 🛡️ Фаза 6: Production-упаковка и шаблонизация (Модули 36-41)

| №   | Модуль                              | Цель                                                | Ключевые навыки                                  | Практика (C# .NET 10 / DevOps)              |
| --- | ----------------------------------- | --------------------------------------------------- | ------------------------------------------------ | ------------------------------------------- |
| 36  | 📡 `Health/Liveness Probes`         | File-based, minimal HTTP/named pipe, startup checks | `health routing`, `liveness sync`, `probe logic` | Воркер сигнализирует «жив» для оркестратора |
| 37  | `dotnet new Template Architecture`  | `.template.config`, placeholders, post-creation     | `template routing`, `symbol sync`, `post logic`  | Создать конфигурацию шаблона                |
| 38  | `Template Packaging & Versioning`   | `.nupack`, `dotnet nuget push`, semver              | `pack routing`, `nuget sync`, `ver logic`        | Упаковать, проверить установку              |
| 39  | `CI/CD for Console Apps`            | Build, TUnit, pack, publish, OTel validation        | `ci routing`, `gate sync`, `publish logic`       | Настроить CI pipeline                       |
| 40  | `Native AOT Readiness`              | Trimming, `JsonSerializerContext`, limits           | `aot routing`, `trim sync`, `compat logic`       | Подготовить код к Native AOT                |
| 41  | `Deployment & Service Registration` | systemd, Windows Service, Docker                    | `deploy routing`, `service sync`, `health logic` | Настроить запуск как службу                 |

### 🌐 Фаза 7: Наблюдаемость в продакшене и аудит (Модули 42-47)

| №   | Модуль                             | Цель                                          | Ключевые навыки                                   | Практика (C# .NET 10 / DevOps)       |
| --- | ---------------------------------- | --------------------------------------------- | ------------------------------------------------- | ------------------------------------ |
| 42  | `OTel Collector Integration`       | Gateway, filtering, batching, routing         | `collector routing`, `filter sync`, `route logic` | Настроить OTel Collector для консоли |
| 43  | `Dashboards & SLOs`                | Grafana, latency histograms, error budgets    | `dashboard routing`, `slo sync`, `alert logic`    | Дашборды для консольного процесса    |
| 44  | `Audit Verification & Compliance`  | Hash verification, tamper alerts, SIEM export | `verify routing`, `tamper sync`, `siem logic`     | Верификация аудита в prod            |
| 45  | `Incident Response & Debugging`    | Correlation ID, trace replay, memory dump     | `incident routing`, `trace sync`, `dump logic`    | Runbooks для отладки                 |
| 46  | `Secret Rotation & Key Management` | Automated rotation, fallback, audit sync      | `rotate routing`, `fallback sync`, `audit logic`  | Ротация секретов без downtime        |
| 47  | `Operations Pipeline`              | Deploy → monitor → recover → audit → improve  | `ops routing`, `sync logic`, `gate logic`         | Автоматизировать ops pipeline        |

### 🚀 Фаза 8: Будущее, оптимизация и сертификация (Модули 48-52)

| №   | Модуль                          | Цель                                             | Ключевые навыки                                   | Практика (C# .NET 10)           |
| --- | ------------------------------- | ------------------------------------------------ | ------------------------------------------------- | ------------------------------- |
| 48  | `AI-Assisted Configuration`     | Auto-tune OTel, validate policies, predict drift | `ai routing`, `tune sync`, `policy logic`         | AI-оптимизатор конфигов         |
| 49  | `PQC & Crypto-Readiness`        | Post-quantum signing, hash upgrades              | `crypto routing`, `pq sync`, `upgrade logic`      | Подготовка криптографии к PQC   |
| 50  | `Future-Proofing & Abstraction` | Interface decoupling, plugin architecture        | `future routing`, `plugin sync`, `version logic`  | Абстракция для расширения       |
| 51  | `Cross-Platform Compatibility`  | Linux/macOS/Windows parity, path/encoding sync   | `platform routing`, `path sync`, `encoding logic` | Идентичное поведение на всех ОС |
| 52  | `Certification & Final Project` | Production-ready template, deploy, sign-off      | `proj routing`, `deploy sync`, `cert logic`       | Сдать финальный проект          |

---

## 📦 ЧТО ПОЛУЧИМ ПО ОКОНЧАНИИ КУРСА

```bash

EP.ConsoleStarter/
├── ConsoleStarter.sln
│
├── src/
│   └── ConsoleStarter/
│       ├── ConsoleStarter.csproj       # .NET 10, AOT-ready, trimming enabled
│       ├── Program.cs                     # IHost builder, DI wiring, GlobalHandler attach, RunAsync
│       ├── AppSettings.cs                 # IOptionsMonitor record + DataAnnotations validation
│       │
│       ├── Core/                          # Бизнес-логика и паттерны
│       │   ├── Result.cs                  # Result<T>, Error, IsSuccess/IsFailure фабрики
│       │   ├── IProcessor.cs              # Интерфейс обработки элемента
│       │   └── DataProcessor.cs           # Реализация: EF Core + Cache + Result + OTel timing
│       │
│       ├── Infrastructure/                # СквозныеConcerns
│       │   ├── GlobalExceptionHandler.cs  # AppDomain/TaskScheduler hooks, OTel flush, exit-code mapper
│       │   ├── AuditPipeline.cs           # Append-only, SHA256 chaining, tamper verification
│       │   ├── HealthProbe.cs             # File-based liveness + minimal readiness check
│       │   └── Cache/                     # Абстракция кэша (Модуль 28)
│       │       ├── ICacheService.cs
│       │       ├── MemoryCacheService.cs  # Default provider с OTel hit/miss/error counters
│       │       └── CacheRegistration.cs   # DI switch: Memory ↔ Redis (по конфигу)
│       │
│       ├── Data/                          # Персистентность
│       │   ├── AppDbContext.cs            # Scoped, AuditedEntities, SaveChanges override (CreatedBy/At)
│       │   ├── Migrations/                # EF Core code-first миграции
│       │   └── DbInitializer.cs           # Programmatic migrations + seed on startup (retry on fail)
│       │
│       ├── Workers/                       # Фоновый конвейер
│       │   └── PipelineWorker.cs          # BackgroundService + Channel<T> + Scope-per-iteration + Cancellation
│       │
│       ├── Cli/                           # Интерфейс командной строки
│       │   └── Commands/
│       │       ├── StartCommand.cs        # Запускает хост и воркера
│       │       └── ProcessCommand.cs      # One-off выполнение + JSON/plain output
│       │
│       └── Diagnostics/                   # Наблюдаемость
│           ├── TelemetryConfig.cs         # OTel Traces/Metrics/Logs setup, CorrelationId propagation
│           └── Metrics.cs                 # Custom counters: cache.hits, processing.duration, queue.depth
│
├── tests/
│   └── ConsoleStarter.Tests/
│       ├── ConsoleStarter.Tests.csproj # TUnit async-first, zero-alloc
│       ├── CacheTests.cs                  # Hit/miss, fallback on error, TTL invalidation
│       ├── PipelineWorkerTests.cs         # Backpressure, graceful shutdown, scope isolation
│       └── GlobalHandlerTests.cs          # Exit code mapping, OTel flush simulation, unobserved tasks
│
├── template/
│   └── .template.config/
│       ├── template.json                  # dotnet new metadata, placeholders, symbols, post-actions
│       └── post-create.sh                 # git init, dotnet restore, dotnet build verification
│
├── .github/
│   └── workflows/
│       └── ci.yml                         # Build → TUnit → Sonar → OTel config validation → pack → publish
│
├── appsettings.json                       # Базовая конфигурация (лог-пути, TTL, retry limits)
├── appsettings.Production.json            # Продакшен-оверрайды (ENV precedence, OTLP endpoint)
└── README.md                              # Setup, hot-reload guide, observability, AOT notes, deploy
```

---

### 🔑 Почему такая структура?

| Слой                                       | Что закрывает                                                                   | Связь с планом   |
| ------------------------------------------ | ------------------------------------------------------------------------------- | ---------------- |
| `Program.cs` + `AppSettings.cs`            | Hot-reload конфигов, DI-скелет, строгий старт                                   | Модули 1-4       |
| `Core/Result.cs` + `Core/DataProcessor.cs` | Разделение бизнес/системных ошибок, явные контракты                             | Модуль 34        |
| `Infrastructure/GlobalExceptionHandler.cs` | Централизованный catch, OTel flush, exit-code mapping                           | Модуль 33-34     |
| `Infrastructure/Cache/`                    | Абстракция, Memory/Redis switch, OTel counters, graceful degradation            | Модуль 28        |
| `Workers/PipelineWorker.cs`                | `Channel<T>`, backpressure, `IServiceScopeFactory` в цикле, `CancellationToken` | Модули 5, 23, 27 |
| `Data/DbInitializer.cs`                    | Programmatic migrations, startup retry, idempotent checks                       | Модуль 22        |
| `Diagnostics/`                             | CorrelationId, OTel setup, custom metrics (cache, queue, duration)              | Модули 7-13      |
| `Cli/Commands/`                            | `System.CommandLine`, `--json`/`--verbose`/`--dry-run`, exit codes              | Модули 31-32     |
| `tests/` (TUnit)                           | Async-first, scope/channel mocking, backpressure simulation, error mapping      | Модули 29-30, 35 |
| `template/` + `ci.yml`                     | `dotnet new` упаковка, AOT-совместимость, security/OTel gates                   | Модули 36-41     |

---

### ✅ Конкретные характеристики финального шаблона

| Аспект             | Что реализовано                                                                    |
| ------------------ | ---------------------------------------------------------------------------------- |
| `Архитектура`      | `IHost` + `BackgroundService` + `Channel<T>` + `IServiceScopeFactory` в цикле      |
| `Observability`    | OpenTelemetry (Traces/Metrics/Logs), `CorrelationId`, OTLP-экспорт, JSON-логи      |
| `Безопасность`     | ENV/DPAPI, `IOptionsMonitor` hot-reload, input sanitization, CSP/FS defaults       |
| `Аудит`            | Append-only log, SHA256 chaining, tamper-evident verification, PII masking         |
| `Обработка ошибок` | `Result<T>` для бизнес-логики, глобальный хэндлер для системных, строгие exit-коды |
| `Тестирование`     | TUnit async-first, source-gen, zero-alloc, mock DI, backpressure simulation        |
| `Доставка`         | `dotnet new ep-console` template, AOT-ready, systemd/Docker/Health probes          |
| `Комплаенс`        | GDPR/152-ФЗ mapping, audit export to SIEM, immutable verification                  |

---

## 💻 Практические примеры кода (C# .NET 10)

### 🔹 1. Ядро: Host + BackgroundService + Channel + Scope + GlobalHandler

```csharp
// Program.cs
using ConsoleStarter.Infrastructure;
using ConsoleStarter.Diagnostics;
using Microsoft.Extensions.Hosting;
using System.CommandLine;

var rootCommand = new RootCommand("Console Starter");
var startCmd = new Command("start", "Run background pipeline");
var processCmd = new Command("process", "One-off execution");
rootCommand.AddCommand(startCmd);
rootCommand.AddCommand(processCmd);

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, svc) =>
    {
        svc.Configure<AppSettings>(ctx.Configuration.GetSection("App"));
        TelemetryConfig.Register(svc, ctx.Configuration);
        svc.AddDbContext<AppDbContext>(...);
        svc.RegisterCache(ctx.Configuration); // Memory or Redis switch
        svc.AddHostedService<PipelineWorker>();
        svc.AddTransient<IProcessor, DataProcessor>();
    });

using var host = builder.Build();
GlobalExceptionHandler.Attach(host,
    host.Services.GetRequiredService<ILogger<Program>>(),
    ex => ex is OperationCanceledException ? 0 : 1);

await rootCommand.InvokeAsync(args); // Делегирует CLI в Host/Worker
```

```csharp
// Workers/PipelineWorker.cs
public class PipelineWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly Channel<WorkItem> _channel = Channel.CreateBounded<WorkItem>(100);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var reader = _channel.Reader;
        while (!stoppingToken.IsCancellationRequested)
        {
            while (await reader.WaitToReadAsync(stoppingToken) && reader.TryRead(out var item))
            {
                await using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var result = await db.ProcessAsync(item, stoppingToken);
                if (!result.IsSuccess)
                    scope.ServiceProvider.GetRequiredService<ILogger>().LogError(result.Error.Message);
            }
        }
    }

    public async ValueTask EnqueueAsync(WorkItem item, CancellationToken ct) =>
        await _channel.Writer.WriteAsync(item, ct);
}
```

**Что закрывает**: Долгоживущий процесс без leak, корректный `Scoped` DI, backpressure, graceful cancellation, интеграция с `Result<T>`.

---

### 🔹 2. Модуль 34: Result Pattern + Global Handler

```csharp
public record Result<T>(bool IsSuccess, T? Value, Error? Error)
{
    public static Result<T> Success(T v) => new(true, v, null);
    public static Result<T> Failure(Error e) => new(false, default, e);
}

public static class GlobalExceptionHandler
{
    public static void Attach(IHost host, ILogger logger, Func<Exception, int> exitMapper)
    {
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            Handle((Exception)e.ExceptionObject, "AppDomain", logger, exitMapper);
        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            Handle(e.Exception, "UnobservedTask", logger, exitMapper);
            e.SetObserved();
        };
    }

    private static void Handle(Exception ex, string src, ILogger log, Func<Exception, int> mapper)
    {
        log.LogCritical(ex, "[UNHANDLED] {Src}: {Msg}", src, ex.Message);
        Environment.Exit(mapper(ex));
    }
}
```

**Стратегия**: Бизнес-ошибки → `Result<T>`. Системные → `throw` → глобальный хэндлер → OTel flush → `exit code`. Никакого `try/catch` повсюду.

---

## ✅ Production-чек-лист

| Аспект          | Вопрос                                                                    | Да/Нет |
| --------------- | ------------------------------------------------------------------------- | ------ |
| `Архитектура`   | `BackgroundService` + `Channel<T>` + `Scope` в цикле?                     | ☐      |
| `Observability` | OTel подключен? `CorrelationId` сквозной?                                 | ☐      |
| `Безопасность`  | Секреты изолированы? `IOptionsMonitor` hot-reload работает?               | ☐      |
| `Аудит`         | Hash-chaining активен? Tamper detection есть?                             | ☐      |
| `Данные`        | Programmatic migrations? Scoped `DbContext`? Polly retry?                 | ☐      |
| `Ошибки`        | `Result<T>` для бизнеса? Global Handler для системных? Exit-коды строгие? | ☐      |
| `Тестирование`  | TUnit async-first? Backpressure тесты?                                    | ☐      |
| `Шаблон`        | `dotnet new` готов? AOT-ready? Health probes?                             | ☐      |
| `Deploy`        | systemd/Docker? Graceful shutdown? Logs rotated?                          | ☐      |
| `Compliance`    | PII masked? Audit verifiable? GDPR mapped?                                | ☐      |

---

## 🎯 Заключение + план действий

> ✅ **Главное правило**: `.NET 10 Console = host-driven + channel-pipeline + scope-per-iteration + otel-by-default + result-strategy + tunit-tested + template-ready`. Не верьте «просто запустил скрипт», верьте: цикл управляется `BackgroundService`, память чистится через `Scope`, ошибки разделены стратегически, конфигурация меняется на лету, аудит неизменяем, шаблон готов. Проектируй конвейер явно, логируй детально, валидируй строго, тестируй асинхронно, упаковывай автоматически.

### 🔹 Что делать прямо сейчас

1. `Создай структуру`: `src/Workers/`, `src/Infrastructure/`, `tests/`, `template/` 2.``Настрой `IHost` + `BackgroundService``: базовый запуск, `Channel<T>`, `Scope` в цикле 3.``Добавь `IOptionsMonitor```: runtime reload, `OnChange` callback
4.``Реализуй `Result<T>` + Global Handler`: разделение бизнес/системных ошибок
5.`Подключи TUnit`: async-first тесты, backpressure simulation
6.`Упакуй шаблон``: `.template.config`, `dotnet pack`, CI gates

📌 **Запомни**: `.NET 10 Console` — это `инженерный конвейер`, а не «скрипт». Начинай с малого: один хост, один канал, один скоуп. Добавляй OTel, безопасность, EF Core, resilience, CLI, шаблон. Каждый шаг — `инвестиция в стабильность`. Документируй: `поток = асинхронный, память = контролируема, ошибки = разделены, наблюдаемость = сквозная, доставка = CI-гейтед` — это твоя `архитектурная конституция` для production-ready .NET 10 Console Engineering.
