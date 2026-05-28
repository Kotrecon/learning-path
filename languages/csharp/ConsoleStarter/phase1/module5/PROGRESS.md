# Прогресс: Модуль 5 — BackgroundService & Cancellation

> Цель: Реализовать фоновый воркер с корректной отменой через CancellationToken.

---

## Секция 1: Базовая реализация BackgroundService

### [x] Задача 5.1: Создать воркер, наследующий BackgroundService с ExecuteAsync

- **Статус:** ✅ Completed
- **Дата выполнения:** 2026-05-27
- **Коммит:** `feat(module5.1): implement PipelineWorker : BackgroundService`
- **Заметки/Наблюдения:**
  - `BackgroundService` снимает рутину по `StartAsync/StopAsync`, оставляя только `ExecuteAsync`
  - `Task.Delay(..., stoppingToken)` гарантирует немедленную реакцию на отмену
  - 💡 Инсайт: воркер — это не поток, а управляемая часть хоста. Доверяй жизненный цикл фреймворку.

---

## Секция 2: Кооперативная отмена через CancellationToken

### [x] Задача 5.2: Реализовать проверку `stoppingToken.IsCancellationRequested` в цикле

- **Статус:** ✅ Completed
- **Дата выполнения:** 2026-05-27
- **Коммит:** `feat(module5.2): implement cooperative cancellation with try/catch/finally`
- **Заметки/Наблюдения:**
  - `while (!token.IsCancellationRequested)` — безопасная точка выхода из цикла
  - `Task.Delay(..., token)` бросает `OperationCanceledException` при отмене, что мгновенно прерывает ожидание
  - `catch (OperationCanceledException)` отделяет штатную остановку от реальных ошибок
  - 💡 Инсайт: кооперативная отмена — это не «прервать», а «договориться завершиться в безопасной точке».

---

## Секция 3: Graceful shutdown и очистка ресурсов

### [x] Задача 5.3: Реализовать логику завершения с освобождением ресурсов

- **Статус:** ✅ Completed
- **Дата выполнения:** 2026-05-27
- **Коммит:** `feat(module5.3): implement graceful shutdown with guaranteed resource cleanup`
- **Заметки/Наблюдения:**
  - `finally` гарантирует выполнение очистки даже при `OperationCanceledException` или других ошибках
  - Ресурсы (соединения, файлы, буферы) должны закрываться _после_ выхода из цикла, но _до_ завершения метода
  - 💡 Инсайт: graceful shutdown — это симметрия. Если ресурс открыт в `try`, он обязан закрыться в `finally`.

---

## Секция 4: Интеграционный тест корректной отмены

### [x] Задача 5.4: Провести тест завершения воркера за <1 секунды после сигнала

- **Статус:** ✅ Completed
- **Дата выполнения:** 2026-05-27
- **Коммит:** `feat(module5.4): add shutdown timing test & verify <1s completion`
- **Заметки/Наблюдения:**
  - `IHostApplicationLifetime.ApplicationStopping.Register()` — точная точка старта замера
  - `Stopwatch` после `RunAsync()` фиксирует полное время освобождения ресурсов хостом
  - 💡 Инсайт: <1 сек — это маркер отсутствия блокирующих вызовов без токена. Docker/K8s ожидают именно такого поведения.

---

## Секция 5: События жизненного цикла через IHostApplicationLifetime

### [x] Задача 5.5: Подписаться на события ApplicationStopping для дополнительной логики

- **Статус:** ✅ Completed
- **Дата выполнения:** 2026-05-27
- **Коммит:** `feat(module5.5): subscribe to IHostApplicationLifetime events for infrastructure hooks`
- **Заметки/Наблюдения:**
  - `ApplicationStopping` срабатывает **до** остановки `IHostedService` — удобно для финального логирования
  - `ApplicationStopped` срабатывает **после** всех сервисов — удобно для метрик завершения
  - ⚠️ Хуки должны быть короткими: тяжёлая работа в них блокирует остановку хоста
  - 💡 Инсайт: `IHostApplicationLifetime` — это orchestration, а `CancellationToken` — это кооперация. Используй оба.

---

## Секция 6: Документация и сдача модуля

### [x] Задача 5.6: Подготовить документацию и открыть PR для сдачи модуля

- **Статус:** ✅ Completed
- **Дата выполнения:** 2026-05-27
- **Коммит:** `docs(module5.6): add README.md with cancellation patterns & acceptance criteria`
- **Заметки/Наблюдения:**
  - Документация фиксирует контракт: кооперативная отмена, гарантированная очистка, события жизненного цикла
  - 💡 Инсайт: документация по shutdown должна явно указывать последовательность логов и критерий <1 сек для интеграционных тестов.

---

## ✅ Критерий приёмки модуля

- `PipelineWorker` наследуется от `BackgroundService` и регистрируется через `AddHostedService`
- `ExecuteAsync(CancellationToken)` содержит цикл с проверкой `stoppingToken.IsCancellationRequested`
- `Task.Delay(..., stoppingToken)` используется для кооперативной отмены
- При нажатии `Ctrl+C` воркер завершается за <1 секунды, логируется `"Worker stopped"`
- `IHostApplicationLifetime.ApplicationStopping` регистрирует дополнительный лог при остановке
- Все изменения зафиксированы в Git с понятной историей коммитов

---

📝 **Правила ведения:** Отмечайте `[x]` только после полной сдачи задачи. В поле `Заметки` пишите только технические факты, ошибки и инсайты. Коммиты указывайте в формате `sha` или `feat(module5): ...`.
