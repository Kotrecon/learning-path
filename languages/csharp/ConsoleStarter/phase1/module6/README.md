# Модуль 6 — 🎯 Baseline Console Pipeline

> Цель: Собрать детерминированный пайплайн `Init → config → DI → run → shutdown → log`, провести parity-тест и подготовить артефакт для деплоя.

---

## 🚀 Быстрый старт

### Сборка и запуск (разработка)

```powershell
# Из корня репозитория
cd src/ConsoleStarter
dotnet run
```

**Ожидаемый вывод:**

- `Application started` — хост поднялся
- `Worker started` — фоновый воркер инициализирован
- Логи формата `[инфо]: [категория] сообщение`

### Остановка

Нажми `Ctrl+C` → приложение корректно завершится:

- `Cancellation requested` — воркер получил сигнал
- `Resource closed` — очистка в `finally`
- `Worker stopped gracefully` — штатный выход

### Тестирование (parity-проверка)

```powershell
# Из корня репозитория
cd tests/ConsoleStarter.Tests
dotnet test
```

**Критерий успеха:** `✅ Parity passed: 5/5` — приложение стабильно стартует 5 раз подряд.

### Публикация (продакшен)

```powershell
# Из корня репозитория
dotnet publish src/ConsoleStarter/ConsoleStarter.csproj -c Release -r win-x64 --self-contained true -o ./publish
```

**Запуск артефакта:**

```powershell
cd ./publish
./ConsoleStarter.exe
```

⚠️ **Важно:** `WorkingDirectory` должен совпадать с папкой `.exe`, либо конфиги резолвятся относительно `Environment.ProcessPath`.

---

## ✅ Критерии приёмки

- [x] `Program.cs` содержит детерминированный порядок: конфигурация → опции → DI → Build → RunAsync
- [x] `ILogger<T>` используется для структурированного логирования (уровни: Debug/Information/Warning/Error)
- [x] Глобальный `try/catch` в `Main` логирует критические ошибки инициализации
- [x] `PipelineWorker` реализует кооперативную отмену через `CancellationToken`
- [x] `finally` гарантирует очистку ресурсов при любой ситуации
- [x] Parity-тест на TUnit проходит 5 запусков подряд без сбоев
- [x] `dotnet publish --self-contained` создаёт рабочий артефакт, запускаемый без установленного .NET SDK
- [x] Документация содержит команды, ожидаемый вывод и производственные заметки

---

## 🔑 Ключевые инсайты

| Концепция       | Инсайт                                                                                                          |
| --------------- | --------------------------------------------------------------------------------------------------------------- |
| **Оркестрация** | Конфигурация должна быть готова ДО регистрации сервисов. Резолв зависимостей — только после `Build()`.          |
| **Логирование** | `ILogger<T>` — не косметика, а наблюдаемость. Категории из типа `T`, уровни фильтруются, шаблоны индексируются. |
| **Отмена**      | `BackgroundService` + `CancellationToken` = кооперативная остановка. `finally` — контракт, а не опция.          |
| **Parity-тест** | 5 запусков подряд — минимум для smoke-проверки. Ловит нестабильность до деплоя.                                 |
| **Публикация**  | `--self-contained` = артефакт для чистой ОС. `WorkingDirectory` обязателен при запуске из сервисов.             |

---

## 🧪 Проверка вручную

1. **Штатный запуск:**

   ```powershell
   cd src/ConsoleStarter && dotnet run
   # Ctrl+C через 3 сек → чистый shutdown
   ```

2. **Парити-тест:**

   ```powershell
   cd tests/ConsoleStarter.Tests && dotnet test
   # Ожидаем: 5/5 runs successful
   ```

3. **Публикация и запуск:**

   ```powershell
   dotnet publish src/ConsoleStarter/ConsoleStarter.csproj -c Release -r win-x64 --self-contained true -o ./publish
   cd ./publish && ./ConsoleStarter.exe
   # Ctrl+C → корректное завершение
   ```

---

## 📦 Артефакты модуля

| Файл                                            | Назначение                                                     |
| ----------------------------------------------- | -------------------------------------------------------------- |
| `src/ConsoleStarter/Program.cs`                 | Финальный пайплайн инициализации и глобальная обработка ошибок |
| `src/ConsoleStarter/Services/PipelineWorker.cs` | Воркер с кооперативной отменой и гарантированной очисткой      |
| `tests/ConsoleStarter.Tests/ParityTests.cs`     | Автоматизированная parity-проверка стабильности запуска        |
| `phase1/module6/README.md`                      | Документация: команды, критерии, инсайты                       |

---

> 💡 **Production-принцип**: Baseline — это не «работает у меня», а «воспроизводимо, наблюдаемо, деплоится». Если запуск плавает — деплой запрещён.

---
