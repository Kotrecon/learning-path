# Модуль 9 — 🔗 Correlation & Context Propagation

> Цель: Настроить сквозной CorrelationId в логах и трейсах, пробросить контекст через Baggage.

---

## 🚀 Быстрый старт

### Сборка и запуск

```powershell
cd src/ConsoleStarter
dotnet run
```

**Ожидаемый вывод:**

- Логи `Program[0]` содержат `LogRecord.TraceId` и `LogRecord.SpanId`
- `CorrelationId` совпадает с `TraceId`
- `Baggage` доступен в асинхронном методе `ProcessAsync`
- Заголовки `traceparent` и `baggage` инжектируются/извлекаются через Propagators

---

## ✅ Критерии приёмки

- [x] Логи содержат `trace_id` и `span_id` автоматически через OTel
- [x] `CorrelationId` извлекается из `Activity.Current.TraceId` и логируется как отдельное поле
- [x] Baggage устанавливается и доступен в асинхронных вызовах через `Baggage.Current`
- [x] Сквозной тест подтверждает, что контекст не теряется между методами
- [x] `module9/README.md` описывает работу с контекстом и пропагаторами
- [x] Все изменения зафиксированы в Git с понятной историей коммитов

---

## 🔑 Ключевые инсайты

| Концепция            | Инсайт                                                                                                                           |
| -------------------- | -------------------------------------------------------------------------------------------------------------------------------- |
| **OTel Logging**     | `builder.Logging.AddOpenTelemetry(logging => logging.AddConsoleExporter())` — правильный способ подключения экспортера для логов |
| **TraceId/SpanId**   | Автоматически добавляются в логи при активном `Activity`. Требуется `using OpenTelemetry.Logs;`                                  |
| **CorrelationId**    | Бизнес-имя для `TraceId`. Извлекается через `Activity.Current?.TraceId.ToString()`                                               |
| **Baggage**          | Кастомные пары key-value в контексте. Пробрасываются через `AsyncLocal` без передачи параметров                                  |
| **Propagators**      | `Inject`/`Extract` сериализуют контекст в HTTP-заголовки (`traceparent`, `baggage`)                                              |
| **W3C TraceContext** | Стандартный формат: `00-{traceId}-{spanId}-{flags}`                                                                              |

---

## 🧪 Проверка вручную

1. **Штатный запуск:**

   ```powershell
   cd src/ConsoleStarter && dotnet run
   # Ожидаем: логи Program[0] с TraceId, CorrelationId, Baggage
   ```

2. **Проверка сквозной корреляции:**
   Убедиться, что три лога содержат одинаковый `TraceId`:
   - `Processing with CorrelationId=...`
   - `Baggage loaded: UserId=123, TenantId=abc`
   - `Inside ProcessAsync: CorrelationId=..., UserId=123`

3. **Проверка Propagators:**
   Убедиться, что заголовки `traceparent` и `baggage` инжектируются и извлекаются корректно.

---

## 📦 Артефакты модуля

| Файл                            | Назначение                                            |
| ------------------------------- | ----------------------------------------------------- |
| `src/ConsoleStarter/Program.cs` | OTel-логирование, CorrelationId, Baggage, Propagators |
| `phase2/module9/README.md`      | Документация модуля                                   |
| `phase2/module9/PROGRESS.md`    | Трекер выполнения задач                               |
| `templates/W3C_TRACE_FORMAT.md` | Справочник по формату W3C TraceContext                |

---

## 📚 Ссылки

- [OpenTelemetry .NET Logs](https://opentelemetry.io/docs/languages/net/logs/)
- [W3C TraceContext](https://www.w3.org/TR/trace-context/)
- [W3C Baggage](https://www.w3.org/TR/baggage/)

---

> 💡 **Production-принцип**: Связь логов и трейсов — фундамент распределённой наблюдаемости. CorrelationId + Baggage + Propagators позволяют отслеживать запрос через границы микросервисов без явной передачи контекста.
