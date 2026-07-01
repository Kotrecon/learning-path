# 📊 Прогресс: Фаза 2 — Логирование и Observability (OTel)

> Сводный трекер по модулям 7–13. Цель: production-ready наблюдаемость через структурированные логи, OpenTelemetry и сквозной контекст.

---

## Модуль 7 — 🔍 Structured Logging Basics

- **Статус:** ✅ Completed
- **PR:** [#7](https://github.com/Kotrecon/learning-path/pull/7)
- **Коммит:** `docs(module7.6): finalize progress tracker for module 7`
- **Артефакты:** `src/ConsoleStarter/Program.cs` (JsonConsoleFormatter + scopes + templates), `phase2/module7/README.md`
- **Ключевые навыки:** `ILogger<T>`, Message Templates, Logging Scopes, JSON-форматирование

---

## Модуль 8 — 📡 OpenTelemetry Integration

- **Статус:** ✅ Completed
- **PR:** [#8](https://github.com/Kotrecon/learning-path/pull/8)
- **Коммит:** `docs(module8.6): add module README and finalize progress tracker`
- **Артефакты:** `src/ConsoleStarter/Program.cs` (AddOpenTelemetry, ActivitySource, Meter, OTLP config), `phase2/module8/README.md`
- **Ключевые навыки:** OTel SDK, Tracing, Metrics, Console/OTLP Exporters

---

## Модуль 9 — 🔗 Correlation & Context Propagation

- **Статус:** ✅ Completed
- **PR:** (ожидается)
- **Коммит:** `docs(module9.6): add module README and finalize progress tracker`
- **Артефакты:** `src/ConsoleStarter/Program.cs` (OTel-логирование, CorrelationId, Baggage, Propagators), `phase2/module9/README.md`, `templates/W3C_TRACE_FORMAT.md`
- **Ключевые навыки:** TraceId/SpanId injection, CorrelationId, Baggage, Propagators, W3C TraceContext

---

## Модуль 10 — 📈 Custom Metrics & Counters

- **Статус:** ⏳ Not Started
- **PR:**
- **Коммит:**
- **Артефакты:** `Meter` instruments (Counter/Histogram/Gauge), `phase2/module10/README.md`
- **Ключевые навыки:** Metric types, Attributes, Views, Naming conventions

---

## Модуль 11 — 🎚️ Log Enrichment & Filtering

- **Статус:** ⏳ Not Started
- **PR:**
- **Коммит:**
- **Артефакты:** Global enrichers, Log filtering config, `phase2/module11/README.md`
- **Ключевые навыки:** Enrichers (MachineName, Version), Filtering by category/level, Dynamic reload

---

## Модуль 12 — 📦 OTel Exporters & Batching

- **Статус:** ⏳ Not Started
- **PR:**
- **Коммит:**
- **Артефакты:** OTLP gRPC config, BatchingProcessor settings, Retry logic, `phase2/module12/README.md`
- **Ключевые навыки:** OTLP protocol, Batching configuration, Retry handling, InMemoryExporter для тестов

---

## Модуль 13 — 🔄 Observability Pipeline

- **Статус:** ⏳ Not Started
- **PR:**
- **Коммит:**
- **Артефакты:** `tests/ObservabilityTests.cs` (Parity tests), `OtelDataValidator`, Pipeline diagram, `phase2/module13/README.md`
- **Ключевые навыки:** Pipeline architecture, State propagation, Validation, Sampling, End-to-end testing

---

> 📝 **Правила ведения:**
>
> 1. Статус меняется на `✅ Completed` только после мержа соответствующего PR.
> 2. В поле `Артефакты` указывай только ключевые файлы, созданные в модуле.
> 3. Коммиты — в формате `feat(moduleX): ...` или `docs(moduleX): ...`.
> 4. PR-ссылки добавляй сразу после создания пул-реквеста.
> 5. Инсайты пиши только в `moduleX/PROGRESS.md`, здесь — только сводка.
