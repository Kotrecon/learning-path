# 📘 Theory: Модуль 1 — Среда и управление зависимостями

## 🎯 Цель модуля

Освоить изоляцию окружений, `uv`/`pyproject.toml`/lockfile, управление зависимостями и воспроизводимые сборки. К концу модуля вы сможете запускать проект в чистой среде одной командой `uv sync --frozen-lockfile`.

---

## 1️⃣ Зачем нужна изоляция окружений

### Проблема глобальных пакетов

- **Глобальный Python** = все пакеты в одном месте (`site-packages`).
- **Проекты конфликтуют**: проект A требует `requests==2.28.0`, проект B — `requests==2.31.0`.
- **Нельзя воспроизвести**: на машине коллеги пакеты другие → код не работает.

### Решение: виртуальные окружения

- **Каждый проект = своя `.venv/`** (изолированный Python + пакеты).
- **Активация**: `source .venv/bin/activate` (Linux/Mac) или `.venv\Scripts\Activate.ps1` (PowerShell).
- **Изоляция**: пакеты не «утекают» между проектами.

### Ограничения `venv` + `pip`

- **Медленно**: `pip install` скачивает и устанавливает по одному.
- **Нет lockfile**: `requirements.txt` = только версии пакетов, НЕ фиксированные хеши.
- **Нет кэша**: каждый раз скачивает пакеты заново.

---

## 2️⃣ `uv` — замена `venv` + `pip` + `pip-tools` + `poetry`

### Что такое `uv`

- **Написан на Rust** → в **10–100× быстрее** `pip`.
- **Единый инструмент**: создание виртуальных окружений, установка пакетов, lockfile, экспорт в `requirements.txt`.
- **Глобальный кэш**: пакеты кэшируются один раз, все проекты используют кэш.

### Ключевые команды `uv`

| Команда                     | Описание                                | Аналог                   |
| --------------------------- | --------------------------------------- | ------------------------ |
| `uv init <project>`         | Создать проект с `pyproject.toml`       | `dotnet new console`     |
| `uv add <package>`          | Добавить зависимость                    | `dotnet add package`     |
| `uv sync`                   | Установить все зависимости из `uv.lock` | `dotnet restore`         |
| `uv sync --frozen-lockfile` | Установить ТОЧНО как в lockfile (CI)    | —                        |
| `uv export`                 | Экспорт в `requirements.txt`            | `pip-compile`            |
| `uv remove <package>`       | Удалить зависимость                     | `dotnet remove package`  |
| `uv pip tree`               | Показать граф зависимостей              | `pip list --format=tree` |
| `uv python pin <version>`   | Зафиксировать версию Python             | —                        |

---

## 3️⃣ `pyproject.toml` — центр метаданных (PEP 621)

### Что такое PEP 621

- **Стандарт Python** (2021) для хранения метаданных в `pyproject.toml`.
- **Заменяет** `setup.py`, `setup.cfg`, `requirements.txt` (частично).
- **Единый формат**: все инструменты (uv, poetry, pip) понимают `pyproject.toml`.

### Структура `pyproject.toml`

```toml
[project]
name = "my-python-project"
version = "0.1.0"
description = "Мой первый проект на uv"
readme = "README.md"
requires-python = ">=3.12"
dependencies = [
    "requests>=2.28.0",
    "pydantic>=2.0.0",
    "structlog>=23.1.0",
]

[dependency-groups]
dev = [
    "pytest>=7.0.0",
    "ruff>=0.4.0",
    "mypy>=1.10.0",
]

[tool.uv]
cache-keys = ["uv.lock"]
```

### Ключевые секции

| Секция                   | Описание                                      |
| ------------------------ | --------------------------------------------- |
| `[project]`              | Метаданные проекта (имя, версия, зависимости) |
| `[project.dependencies]` | Основные зависимости (prod)                   |
| `[dependency-groups]`    | Группы зависимостей (dev, test, docs)         |
| `[tool.uv]`              | Настройки uv (кэш, поведение)                 |

---

## 4️⃣ Lockfile (`uv.lock`) — детерминизм сборок

### Что такое lockfile

- **Фиксирует точные версии** всех пакетов + хеши.
- **Генерируется** автоматически при `uv add`.
- **Коммитится** в Git (в отличие от `.venv/`).

### Почему lockfile важен

| Без lockfile                                      | С lockfile                                                  |
| ------------------------------------------------- | ----------------------------------------------------------- |
| `requests==2.28.0` → может обновиться до 2.31.0   | `requests==2.28.0` → всегда 2.28.0 (даже если вышла 2.32.0) |
| На машине коллеги пакеты другие → код не работает | На любой машине пакеты одинаковые → код работает            |
| Нельзя воспроизвести баг                          | Можно воспроизвести баг (те же версии)                      |

### `--frozen-lockfile` в CI

- **Запрещает обновление** lockfile без явного `uv add`.
- **Если lockfile устарел** → `uv sync --frozen-lockfile` падает с ошибкой.
- **CI должен использовать** `--frozen-lockfile` (гарантия детерминизма).

---

## 5️⃣ Зависимости: prod vs dev

### Основные зависимости (prod)

- **Нужны для запуска** приложения.
- **Примеры**: `requests`, `pydantic`, `structlog`, `httpx`, `fastapi`.
- **Добавление**: `uv add requests pydantic`.

### Зависимости для разработки (dev)

- **Нужны ТОЛЬКО для разработки** (тесты, линтинг, форматирование).
- **Примеры**: `pytest`, `ruff`, `mypy`, `deptry`, `pre-commit`.
- **Добавление**: `uv add --group dev pytest ruff mypy`.

### Почему разделять

- **Меньше пакетов в prod** → быстрее установка, меньше уязвимостей.
- **Чистота**: в CI prod-окружение не содержит тестовых библиотек.

---

## 6️⃣ Инструменты: `deptry`, кэш CI, `.python-version`

### `deptry` — поиск unused/missing imports

- **Находит**:
  - `unused`: пакет установлен, но не используется в коде.
  - `missing`: импортируется, но не установлен.
- **Запуск**: `uv run deptry .`.
- **Результат**: очистка зависимостей → меньше уязвимостей, быстрее установка.

### Кэш CI

- **uv кэширует пакеты глобально** (`~/.cache/uv`).
- **В CI**: кэшировать `~/.cache/uv` → установка быстрее.
- **GitHub Actions**:

  ```yaml
  - uses: actions/cache@v4
    with:
      path: ~/.cache/uv
      key: ${{ runner.os }}-uv-${{ hashFiles('uv.lock') }}
  ```

### `.python-version` — фиксация версии интерпретатора

- **Файл**: `.python-version` в корне проекта.
- **Содержит**: `3.12.4` (точная версия Python).
- **Команда**: `uv python pin 3.12.4`.
- **Зачем**: на машине коллеги тоже Python 3.12.4 → нет проблем с совместимостью.

---

## 7️⃣ Параллели с C# / .NET

| Python                      | .NET                                                         | Описание                        |
| --------------------------- | ------------------------------------------------------------ | ------------------------------- |
| `uv init`                   | `dotnet new console`                                         | Создать проект                  |
| `uv add <package>`          | `dotnet add package <package>`                               | Добавить зависимость            |
| `uv.lock`                   | `packages.lock.json`                                         | Фиксация версий                 |
| `uv sync --frozen-lockfile` | `dotnet restore --locked-mode`                               | Установить точно как в lockfile |
| `pyproject.toml`            | `.csproj` + `NuGet.config`                                   | Метаданные проекта              |
| `dependency-groups.dev`     | `<PackageReference Condition="'$(Configuration)'=='Debug'">` | dev-зависимости                 |
| `deptry`                    | `dotnet list package --outdated`                             | Поиск устаревших/лишних пакетов |

---

## 8️⃣ Типичные ошибки и как их избежать

| Ошибка                          | Причина                         | Решение                                              |
| ------------------------------- | ------------------------------- | ---------------------------------------------------- |
| `.venv/` в Git                  | `.venv/` = локальное окружение  | Добавить `.venv/` в `.gitignore`                     |
| `uv.lock` не в Git              | lockfile = детерминизм          | **Всегда коммитить** `uv.lock`                       |
| Ручное редактирование `uv.lock` | lockfile = автоматически        | Никогда не править вручную, только `uv add/remove`   |
| `pip install` вместо `uv sync`  | смешивание инструментов         | Всегда `uv sync`, НИКОГДА `pip install` в проекте uv |
| Нет `.python-version`           | разные версии Python на машинах | `uv python pin 3.12` → коммитить `.python-version`   |

---

## 9️⃣ Ключевые выводы модуля

1. **`uv` = venv + pip + pip-tools + poetry в одном** → быстрее, проще, надёжнее.
2. **`pyproject.toml` = центр метаданных** (PEP 621), не `setup.py`.
3. **`uv.lock` = контракт** → всегда коммитить, никогда не править вручную.
4. **`--frozen-lockfile` в CI** → гарантия детерминизма.
5. **Разделять prod vs dev зависимости** → меньше уязвимостей, быстрее установка.
6. **`deptry` ловит 90% import-багов** до runtime.
7. **`.python-version` = фиксация интерпретатора** → нет проблем с совместимостью.

---

## 🔟 Ссылки на документацию

| Ресурс         | Ссылка                                                    |
| -------------- | --------------------------------------------------------- |
| PEP 621        | `https://peps.python.org/pep-0621/`                       |
| uv docs        | `https://docs.astral.sh/uv/`                              |
| uv install     | `https://docs.astral.sh/uv/getting-started/installation/` |
| pyproject.toml | `https://docs.astral.sh/uv/concepts/projects/config/`     |
| lockfile       | `https://docs.astral.sh/uv/concepts/projects/lockfile/`   |
| deptry         | `https://deptry.com/`                                     |
