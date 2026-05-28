# Модуль 5 — BackgroundService & Cancellation

> Цель: Реализовать фоновый воркер с корректной отменой через CancellationToken.

---

## Секция 1: Базовая реализация BackgroundService

## Задача 5.1: Создать воркер, наследующий BackgroundService с ExecuteAsync

**Описание:** Изучить `BackgroundService` как стандарт для долгоживущих фоновых задач в .NET 10. Реализовать `ExecuteAsync(CancellationToken)` с циклом, проверяющим `CancellationToken`. Зарегистрировать сервис через `AddHostedService`.

**Способы достижения:**

- Создать класс `PipelineWorker : BackgroundService` с переопределением `ExecuteAsync(CancellationToken)`.
- В цикле выполнить полезную работу и `await Task.Delay(1000, stoppingToken)`.
- В `Program.cs` добавить: `builder.Services.AddHostedService<PipelineWorker>()`.
- Закоммитить: `git add .` затем `git commit -m "feat(module5): implement BackgroundService base"`.

**Результат:** Класс `PipelineWorker.cs`, лог `"Worker started"` при запуске, коммит с базовой реализацией в репозитории.

---

## Секция 2: Кооперативная отмена через CancellationToken

## Задача 5.2: Реализовать проверку `stoppingToken.IsCancellationRequested` в цикле

**Описание:** Понять принцип кооперативной отмены: воркер сам проверяет токен и завершает работу. Использовать `Task.Delay` с токеном для немедленной реакции на отмену. Обернуть цикл в `try/catch` для `OperationCanceledException`.

**Способы достижения:**

- В цикле добавить условие: `while (!stoppingToken.IsCancellationRequested)`.
- Использовать `await Task.Delay(1000, stoppingToken)` внутри цикла.
- Добавить `catch (OperationCanceledException)` для чистой обработки отмены.
- Закоммитить: `git commit -m "feat(module5): implement cooperative cancellation"`.

**Результат:** Код цикла с проверкой токена, лог `"Cancellation requested, exiting gracefully"` при Ctrl+C, коммит с обработкой отмены в репозитории.

---

## Секция 3: Graceful shutdown и очистка ресурсов

## Задача 5.3: Реализовать логику завершения с освобождением ресурсов

**Описание:** При получении сигнала отмены завершить текущую операцию, освободить ресурсы (закрыть соединения, файлы), записать финальные логи. Использовать `finally`-блок для гарантированной очистки.

**Способы достижения:**

- Обернуть основной цикл в `try/finally` или `try/catch/finally`.
- В `finally` добавить логирование `"Worker stopped"` и очистку ресурсов.
- Протестировать: запустить → нажать Ctrl+C → увидеть последовательность логов.
- Закоммитить: `git commit -m "feat(module5): implement graceful shutdown logic"`.

**Результат:** Лог последовательности: `"Cancellation requested"` → `"Cleaning up"` → `"Worker stopped"`, коммит с graceful shutdown в истории репозитория.

---

## Секция 4: Интеграционный тест корректной отмены

## Задача 5.4: Провести тест завершения воркера за <1 секунды после сигнала

**Описание:** Собрать финальный `PipelineWorker` с циклом, проверкой токена и обработкой отмены. Замерить время между нажатием Ctrl+C и полным завершением процесса. Убедиться, что ресурсы освобождены.

**Способы достижения:**

- Собрать финальный код `PipelineWorker` с логированием на каждом этапе.
- Запустить приложение, через 5 секунд нажать Ctrl+C, замерить время завершения.
- Проверить, что после завершения нет утечек (открытых файлов, соединений).
- Закоммитить: `git add .` затем `git commit -m "feat(module5): finalize BackgroundService with cancellation"`.

**Результат:** Финальный код `PipelineWorker.cs`, лог успешного завершения <1 сек, коммит с финализацией в репозитории.

---

## Секция 5: События жизненного цикла через IHostApplicationLifetime

## Задача 5.5: Подписаться на события ApplicationStopping для дополнительной логики

**Описание:** Использовать `IHostApplicationLifetime` для регистрации дополнительных действий при остановке приложения. Добавить логирование или уведомление внешних систем при событии `ApplicationStopping`.

**Способы достижения:**

- Получить сервис: `var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>()`.
- Зарегистрировать callback: `lifetime.ApplicationStopping.Register(() => _logger.LogInformation("Stopping..."))`.
- Запустить приложение, нажать Ctrl+C, увидеть лог события перед завершением воркера.
- Закоммитить: `git commit -m "chore(module5): add IHostApplicationLifetime events"`.

**Результат:** Лог `"Application is stopping"` при нажатии Ctrl+C, коммит с подпиской на события в репозитории.

---

## Секция 6: Документация и сдача модуля

## Задача 5.6: Подготовить документацию и открыть PR для сдачи модуля

**Описание:** Создать `README.md` с примером `PipelineWorker` и инструкцией по проверке корректной отмены. Провести финальный тест: запуск → работа → Ctrl+C → завершение <1 сек. Открыть Pull Request для сдачи.

**Способы достижения:**

- Создать `module5/README.md` с кодом воркера и шагами проверки отмены.
- Прогнать финальный тест и зафиксировать время завершения в логе.
- Создать ветку `feature/module5-complete`, запушить и открыть PR с чек-листом.
- Приложить к PR лог временных меток и подтверждение отсутствия утечек.

**Результат:** Документированный модуль в `README.md`, открыт PR с меткой `"Модуль 5 завершён"`, воркер завершается корректно, все критерии подтверждены.

---

## ✅ Критерий приёмки

- `PipelineWorker` наследуется от `BackgroundService` и регистрируется через `AddHostedService`.
- `ExecuteAsync(CancellationToken)` содержит цикл с проверкой `stoppingToken.IsCancellationRequested`.
- `Task.Delay(..., stoppingToken)` используется для кооперативной отмены.
- При нажатии Ctrl+C воркер завершается за <1 секунды, логируется `"Worker stopped"`.
- `IHostApplicationLifetime.ApplicationStopping` регистрирует дополнительный лог при остановке.
- Все изменения зафиксированы в Git с понятной историей коммитов.
