# Прогресс: Модуль 9 — Correlation & Context Propagation

> Цель: Настроить сквозной CorrelationId в логах и трейсах, пробросить контекст через Baggage.

---

## Секция 1: TraceId & SpanId в логах

### [x] Задача 9.1: Включить автоматическую инъекцию TraceId/SpanId в логи через OTel

- **Статус:** ✅ Completed
- **Дата выполнения:** 2026-07-01
- **Коммит:** `feat(module9.1): enable TraceId/SpanId injection in logs`
- **Заметки/Наблюдения:**
  - `AddConsoleExporter()` для логов работает через `builder.Logging.AddOpenTelemetry(logging => logging.AddConsoleExporter())`, а НЕ через `IServiceCollection.AddOpenTelemetry().WithLogs()`
  - Обязателен `using OpenTelemetry.Logs;` — без него компилятор не видит extension method
  - Логи `Program[0]` внутри Activity содержат `LogRecord.TraceId` и `LogRecord.SpanId`
  - Логи вне Activity (PipelineWorker, ConfigMonitor) **не** содержат trace-контекст
  - 💡 Инсайт: Экспортеры для логов настраиваются через `builder.Logging`, а не через `builder.Services`

---

## Секция 2: CorrelationId — бизнес-идентификатор запроса

### [x] Задача 9.2: Реализовать извлечение и логирование CorrelationId из Activity

- **Статус:** ✅ Completed
- **Дата выполнения:** 2026-07-01
- **Коммит:** `feat(module9.2): implement CorrelationId logging from Activity`
- **Заметки/Наблюдения:**
  - `Activity.Current?.TraceId.ToString()` возвращает hex-строку 128 бит
  - Fallback: `Guid.NewGuid().ToString("N")` когда Activity нет
  - CorrelationId совпадает с `LogRecord.TraceId` — подтверждение сквозной связи
  - Нужен `using System.Diagnostics;` для доступа к `Activity.Current`
  - 💡 Инсайт: CorrelationId = бизнес-имя для TraceId. В проде можно добавить префикс/формат

---

## Секция 3: Baggage — кастомный контекст в трейсах

### [x] Задача 9.3: Настроить передачу кастомных данных через Baggage API

- **Статус:** ✅ Completed
- **Дата выполнения:** 2026-07-01
- **Коммит:** `feat(module9.3): use Baggage for custom context propagation`
- **Заметки/Наблюдения:**
  - `Baggage.SetBaggage("user.id", "123")` — установка значений
  - `Baggage.Current.GetBaggage("user.id")` — чтение значений
  - Значения доступны в том же контексте выполнения
  - Нужен `using OpenTelemetry.Context;`
  - 💡 Инсайт: Baggage пробрасывается через AsyncLocal, работает в async-методах без передачи параметров

---

## Секция 4: Сквозная проверка корреляции контекста

### [x] Задача 9.4: Провести интеграционный тест сквозного проброса CorrelationId и Baggage

- **Статус:** ✅ Completed
- **Дата выполнения:** 2026-07-01
- **Коммит:** `feat(module9.4): verify end-to-end correlation propagation`
- **Заметки/Наблюдения:**
  - Три лога с одинаковым `TraceId`: основной код, Baggage, ProcessAsync
  - `CorrelationId` и `UserId` доступны в `ProcessAsync` без передачи параметров
  - `ExecutionContext` корректно копируется при `await`
  - 💡 Инсайт: AsyncLocal + OTel = сквозная корреляция без явной передачи контекста

---

## Секция 5: Propagators — сериализация контекста для сети

### [x] Задача 9.5: Исследовать механизмы пропагации контекста через границы процессов

- **Статус:** ✅ Completed
- **Дата выполнения:** 2026-07-01
- **Коммит:** `chore(module9.5): explore manual context propagation via Propagators`
- **Заметки/Наблюдения:**
  - `Propagators.DefaultTextMapPropagator.Inject()` — записывает `traceparent` и `baggage` в словарь
  - `Propagators.DefaultTextMapPropagator.Extract()` — читает заголовки и восстанавливает `ActivityContext`
  - Формат `traceparent`: `00-{traceId}-{spanId}-{flags}`
  - `PropagationContext` принимает `Activity.Current.Context`, а не `Activity`
  - Геттер для `Extract` возвращает `IEnumerable<string>`, не `string`
  - 💡 Инсайт: Propagators = sérializer/deserializer контекста для распределённой трассировки

---

## Секция 6: Документация и сдача модуля

### [ ] Задача 9.6: Подготовить документацию и открыть PR для сдачи модуля

- **Статус:** ⏳
- **Дата выполнения:**
- **Коммит:**
- **Заметки/Наблюдения:**

---

## ✅ Критерий приёмки модуля

- [x] Логи содержат `trace_id` и `span_id` автоматически через OTel
- [x] `CorrelationId` извлекается из `Activity.Current.TraceId` и логируется как отдельное поле
- [x] Baggage устанавливается и доступен в асинхронных вызовах через `Baggage.Current`
- [x] Сквозной тест подтверждает, что контекст не теряется между методами
- [ ] `module9/README.md` описывает работу с контекстом и пропагаторами
- [ ] Все изменения зафиксированы в Git с понятной историей коммитов

---

📝 **Правила ведения:** Отмечайте `[x]` только после полной сдачи задачи. В поле `Заметки` пишите только технические факты, ошибки и инсайты. Коммиты указывайте в формате `sha` или `feat(module9): ...`.
