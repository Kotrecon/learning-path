# 📘 Фаза 1 (Python). Фундамент и современный тулинг (Модули 1-6)

**Общая цель фазы:** Научиться настраивать современное Python-окружение, писать код на синтаксисе 3.10+, автоматизировать линтинг/типизацию, управлять конфигурацией/логами, создавать CLI-утилиты и выстраивать отказоустойчивые retry-паттерны.

---

## 📘 Модуль 1 — Среда и управление зависимостями

**Цель:** Освоить изоляцию окружений, `uv`/`poetry`/`pip-tools`, `pyproject.toml` и воспроизводимые сборки.

### 📅 Стандартный ежедневный формат (60 мин)

| Этап                     | Время  | Что делать                                                        |
| ------------------------ | ------ | ----------------------------------------------------------------- |
| 📖 Теория / Разбор       | 15 мин | Чтение PEP 621, `uv` docs, lockfile spec, разрешение зависимостей |
| 💻 Практика              | 25 мин | Инициализация проекта, добавление пакетов, генерация lock-файлов  |
| 🔍 Отладка / Инструменты | 15 мин | `uv pip tree`, `deptry`, `uv export`, VS Code Python extension    |
| 📝 Фиксация              | 5 мин  | Коммит в Git, запись инсайтов в `PROGRESS.md`, план на завтра     |

### 📅 День 1: Virtual envs, `uv` vs `venv`, `pyproject.toml` (PEP 621)

| Этап           | Время  | Действие                                                                                                                                                                                             |
| -------------- | ------ | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 📖 Теория      | 15 мин | Прочитать: [PEP 621](https://peps.python.org/pep-0621/). Понять: `uv` — замена `venv`+`pip`, написан на Rust, в 10-100x быстрее. `pyproject.toml` заменяет `setup.py`.                               |
| 💻 Практика    | 25 мин | Создать проект: `powershell<br>uv init my-python-project && cd my-python-project<br># pyproject.toml уже создан<br>uv add requests pydantic structlog<br>uv sync`<br>Проверить `.venv/` и `uv.lock`. |
| 🔍 Инструменты | 15 мин | **Terminal**: `uv pip tree` → увидеть граф зависимостей. Проверить: `uv.lock` фиксирует точные версии и хеши.                                                                                        |
| 📝 Фиксация    | 5 мин  | Коммит: `feat(module1): init project with uv & pyproject.toml`. Записать: `uv sync` детерминирован, всегда используйте его в CI.                                                                     |

📦 **Артефакт дня:** `pyproject.toml`, `uv.lock`, скриншот `uv pip tree`.

### 📅 День 2: Lock-файлы, разрешение конфликтов, `--frozen-lockfile`

| Этап           | Время  | Действие                                                                                                                                                        |
| -------------- | ------ | --------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 📖 Теория      | 15 мин | Разобрать: `uv.lock` = аналог `poetry.lock`/`package-lock.json`. Конфликты разрешаются SAT-solver'ом. `--frozen-lockfile` запрещает апдейт без явного `uv add`. |
| 💻 Практика    | 25 млн | Добавить конфликтующие зависимости: `uv add fastapi "httpx>=0.25,<0.27"`. Увидеть разрешение или ошибку. Зафиксировать: `uv sync --frozen-lockfile`.            |
| 🔍 Инструменты | 15 мин | **Terminal**: `uv sync --frozen-lockfile` → fail, если lock устарел. Проверить: CI должен использовать этот флаг.                                               |
| 📝 Фиксация    | 5 мин  | Коммит: `chore(module1): enforce frozen lockfile in workflow`. Записать: lockfile = контракт, не редактируйте вручную.                                          |

📦 **Артефакт дня:** Скриншот `uv sync --frozen-lockfile` проверки.

### 📅 День 3: `uv export`, `pip-tools`, reproducible builds

| Этап           | Время  | Действие                                                                                                                                                            |
| -------------- | ------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 📖 Теория      | 15 мин | Понять: `uv export` генерирует `requirements.txt` для legacy-пайплайнов. `pip-compile` (из `pip-tools`) делает то же, но медленнее. `uv` кэширует пакеты глобально. |
| 💻 Практика    | 25 млн | Экспортировать: `uv export --no-hashes > requirements.txt`. Сравнить с `pip-compile pyproject.toml`. Убедиться: `uv` быстрее, `requirements.txt` детерминирован.    |
| 🔍 Инструменты | 15 мин | **VS Code**: настроить интерпретатор на `.venv/Scripts/python.exe`. Проверить: автодополнение работает, типы резолвятся.                                            |
| 📝 Фиксация    | 5 мин  | Коммит: `chore(module1): add requirements.txt export for legacy CI`. Записать: `requirements.txt` = fallback для Docker/CI, основной источник = `uv.lock`.          |

📦 **Артефакт дня:** `requirements.txt`, скриншот VS Code Python env.

### 📅 День 4: Практика — Создать проект с `uv`, добавить зависимости, сгенерировать lock

| Этап           | Время  | Действие                                                                                                                                              |
| -------------- | ------ | ----------------------------------------------------------------------------------------------------------------------------------------------------- |
| 📖 Теория      | 15 мин | Повторить чек-лист: 1) `uv init`. 2) `pyproject.toml` (metadata, dependencies, scripts). 3) `uv add`. 4) `uv sync`. 5) `--frozen-lockfile` в CI.      |
| 💻 Практика    | 25 мин | Собрать `starter-project/`: добавить `dev` группу `uv add --group dev pytest ruff mypy`. Настроить `[tool.uv]` кэш. Запустить `uv sync --all-groups`. |
| 🔍 Инструменты | 15 мин | **Terminal**: `uv pip list` → увидеть prod + dev пакеты. Проверить: `.venv/` не коммитится, `uv.lock` коммитится.                                     |
| 📝 Фиксация    | 5 мин  | Коммит: `feat(module1): scaffold production-ready uv project`. Сохранить `audit/uv-setup-checklist.md`.                                               |

📦 **Артефакт дня:** `starter-project/` структура, скриншот `uv pip list`.

### 📅 День 5: Инструменты: `deptry`, кэш CI, `.python-version`

| Этап           | Время  | Действие                                                                                                                                            |
| -------------- | ------ | --------------------------------------------------------------------------------------------------------------------------------------------------- |
| 📖 Теория      | 15 мин | Разобрать: `deptry` ищет unused/missing imports. `.python-version` фиксирует версию интерпретатора. `uv python pin 3.12`. `uv python install 3.12`. |
| 💻 Практика    | 25 млн | Установить: `uv add --dev deptry`. Запустить: `uv run deptry .`. Увидеть unused imports. Добавить `.python-version` с `3.12.4`.                     |
| 🔍 Инструменты | 15 мин | **Terminal**: `uv python install --default 3.12` → проверить `python --version`. Убедиться: проект изолирован от системного Python.                 |
| 📝 Фиксация    | 5 мин  | Коммит: `chore(module1): add deptry & python version pin`. Записать: `deptry` ловит 90% import-багов до runtime.                                    |

📦 **Артефакт дня:** `.python-version`, скриншот `deptry` отчёта.

### 📅 День 6: Итог недели — Документация и чек-лист

| Этап           | Время  | Действие                                                                                                                                                                             |
| -------------- | ------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| 📖 Теория      | 15 мин | Документация Environment: `uv`, `pyproject.toml`, lockfiles, `.python-version`, `deptry`, CI-кэш. Архитектурное решение: окружение = воспроизводимое состояние, не ручная настройка. |
| 💻 Практика    | 25 мин | Создать `module1/README-env-deps.md`. Собрать файлы. Прогн `uv sync --frozen-lockfile`.                                                                                              |
| 🔍 Инструменты | 15 мин | Финальная проверка: 0 missing deps, lockfile валиден, Python версия зафиксирована, `deptry` чист.                                                                                    |
| 📝 Фиксация    | 5 мин  | Коммит: `docs(module1): add Environment & Dependencies guidelines`. PR: "Модуль 1 завершён".                                                                                         |

✅ **Критерий приёмки Модуля 1:** Проект создан через `uv init`, `pyproject.toml` соответствует PEP 621, `uv.lock` сгенерирован и коммитится, `--frozen-lockfile` работает, `.python-version` зафиксирован, `deptry` не находит ошибок, документация готова.

---

## 📘 Модуль 2 — Современный синтаксис Python 3.10+

**Цель:** Писать выразительный, типобезопасный код с `match/case`, walrus-оператором, dataclasses и новыми аннотациями типов.

### 📅 День 1: `match/case`, структурное сопоставление, guards

| Этап           | Время  | Действие                                                                                                                                                                                                                                                                 |
| -------------- | ------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| 📖 Теория      | 15 мин | Прочитать: `[Structural Pattern Matching](https://docs.python.org/3/reference/compound_stmts.html#match)`. Понять: `match` работает с типами, кортежами, литералами. `case _` = default. `if` после `case` = guard.                                                      |
| 💻 Практика    | 25 мин | Создать `module2/match_basics.py: python def parse_response(status: int, data: dict):  match status:  case 200: return data  case 400 case 422: raise ValueError(f"Bad input: {data}")  case 500 if "retry" in data: return "retry" case: raise RuntimeError("Unknown")` |
| 🔍 Инструменты | 15 мин | **VS Code Debugger**: пошагово пройти `match`. Проверить: `case _` ловит всё, guard срабатывает только при условии.                                                                                                                                                      |
| 📝 Фиксация    | 5 мин  | Коммит: `feat(module2): implement match/case parsing`. Записать: `match` ≠ `switch`, он проверяет структуру, не только значение.                                                                                                                                         |

📦 **Артефакт дня:** `match_basics.py`, скриншот отладки.

### 📅 День 2: Walrus operator `:=`, область видимости, читаемость

| Этап           | Время  | Действие                                                                                                                                                                                                                        |
| -------------- | ------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 📖 Теория      | 15 мин | Разобрать: `:=` присваивает внутри выражения. Область = функция/модуль. Полезен в `while`, `if`, list comprehensions. Не злоупотребляйте.                                                                                       |
| 💻 Практика    | 25 млн | Создать `walrus_usage.py`: `python<br># Legacy:<br>data = get_data()<br>if data: process(data)<br># Modern:<br>if (data := get_data()): process(data)<br><br># Comprehension:<br>[x for x in items if (val := process(x)) > 0]` |
| 🔍 Инструменты | 15 мин | **Python REPL**: `walrus_usage.py`. Проверить: `val` доступен после comprehension. Убедиться: не используйте в сложных вложенных выражениях.                                                                                    |
| 📝 Фиксация    | 5 мин  | Коммит: `feat(module2): apply walrus operator for concise logic`. Записать: `:=` сокращает дублирование, но снижает читаемость при >1 использовании в строке.                                                                   |

📦 **Артефакт дня:** `walrus_usage.py`, скриншот REPL вывода.

### 📅 День 3: `@dataclass`, `frozen=True`, `field()`, `__post_init__`

| Этап           | Время  | Действие                                                                                                                                                                                                                                                                                                       |
| -------------- | ------ | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 📖 Теория      | 15 мин | Понять: `@dataclass` генерирует `__init__`, `__repr__`, `__eq__`. `frozen=True` = immutable (hashable). `field(default_factory=...)` для mutable defaults. `__post_init__` для валидации.                                                                                                                      |
| 💻 Практика    | 25 млн | Создать `modern_models.py`: `python<br>from dataclasses import dataclass, field<br>@dataclass(frozen=True)<br>class User:<br>    id: str<br>    name: str<br>    tags: list[str] = field(default_factory=list)<br>    def __post_init__(self):<br>        if not self.name: raise ValueError("Name required")` |
| 🔍 Инструменты | 15 мин | **Terminal**: `python -c "from modern_models import User; u = User('1', 'Dev'); print(u)"`. Проверить: `u.name = "X"` → `FrozenInstanceError`.                                                                                                                                                                 |
| 📝 Фиксация    | 5 мин  | Коммит: `feat(module2): implement frozen dataclasses with post_init`. Записать: `frozen=True` делает dataclass хэшируемым → можно использовать в `set`/`dict` ключах.                                                                                                                                          |

📦 **Артефакт дня:** `modern_models.py`, скриншот `FrozenInstanceError`.

### 📅 День 4: Практика — Переписать классы на dataclasses, добавить match для парсинга

| Этап           | Время  | Действие                                                                                                                                                                                                                                               |
| -------------- | ------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| 📖 Теория      | 15 мин | Повторить чек-лист: 1) Удалить ручные `__init__`/`__repr__`. 2) Добавить `@dataclass(frozen=True)`. 3) `match` для dispatching. 4) `field(default_factory=...)`. 5) Типизация.                                                                         |
| 💻 Практика    | 25 мин | Взять `legacy_classes.py` (ручные классы с `__init__`), рефакторить: `python<br>@dataclass(frozen=True)<br>class Event: type: str; payload: dict<br>def handle(e: Event): match e.type: case "click": process_click(e.payload) ...`<br>Протестировать. |
| 🔍 Инструменты | 15 мин | **pytest**: написать тесты на immutable behavior и match routes. Увидеть: код короче на 40%, быстрее, типобезопаснее.                                                                                                                                  |
| 📝 Фиксация    | 5 мин  | Коммит: `refactor(module2): migrate legacy classes to dataclasses + match`. Сохранить `audit/syntax-refactor-diff.md`.                                                                                                                                 |

📦 **Артефакт дня:** Рефакторенный `modern_events.py`, отчёт pytest.

### 📅 День 5: Инструменты: `TypeAlias`, `typing.Self`, `assert_never`

| Этап           | Время  | Действие                                                                                                                                                                                                                                                               |
| -------------- | ------ | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 📖 Теория      | 15 мин | Разобрать: `TypeAlias` = читаемые псевдонимы. `Self` = возврат текущего типа. `assert_never(x)` = compile-time exhaustiveness check для `match`.                                                                                                                       |
| 💻 Практика    | 25 млн | Добавить в `models.py`: `python<br>from typing import Self, assert_never<br>JsonDict = dict[str, object]<br>def chain(self: Self, val: int) -> Self: return type(self)(**self.__dict__, val=val)<br>def process(e: Event): match e: case ...; case _: assert_never(e)` |
| 🔍 Инструменты | 15 мин | **mypy**: `mypy models.py --strict`. Увидеть: `assert_never` гарантирует, что `match` покрывает все кейсы. Проверить: `Self` резолвится корректно.                                                                                                                     |
| 📝 Фиксация    | 5 мин  | Коммит: `chore(module2): add modern typing aliases & exhaustiveness`. Записать: `assert_never` = компиляторная страховка, не runtime.                                                                                                                                  |

📦 **Артефакт дня:** Обновлённый `models.py`, скриншот `mypy` вывода.

### 📅 День 6: Итог недели — Документация и чек-лист

| Этап           | Время  | Действие                                                                                                                                                                                         |
| -------------- | ------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| 📖 Теория      | 15 мин | Документация Syntax: `match/case`, `:=`, `@dataclass(frozen=True)`, `TypeAlias`, `Self`, `assert_never`. Архитектурное решение: современная синтаксис = читаемость + immutability + type safety. |
| 💻 Практика    | 25 мин | Создать `module2/README-modern-python.md`. Собрать файлы. Прогн `mypy --strict`.                                                                                                                 |
| 🔍 Инструменты | 15 мин | Финальная проверка: 0 legacy-классов, `match` покрывает все кейсы, `assert_never` работает, `frozen=True` enforced.                                                                              |
| 📝 Фиксация    | 5 мин  | Коммит: `docs(module2): add Modern Syntax guidelines`. PR: "Модуль 2 завершён".                                                                                                                  |

✅ **Критерий приёмки Модуля 2:** Legacy-классы заменены на `@dataclass(frozen=True)`, `match/case` используется для парсинга/dispatching, walrus-оператор применён осознанно, `assert_never` обеспечивает exhaustiveness, `mypy --strict` проходит, документация готова.

---

## 📘 Модуль 3 — Код-стайл и статический анализ

**Цель:** Автоматизировать форматирование, линтинг и типизацию через `ruff`, `black`, `mypy`/`pyright` и `pre-commit`.

### 📅 День 1: `ruff` vs `black`/`flake8`/`isort`, скорость, конфигурация

| Этап           | Время  | Действие                                                                                                                                                                                                                      |
| -------------- | ------ | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 📖 Теория      | 15 мин | Прочитать: [Ruff Docs](https://docs.astral.sh/ruff/). Понять: `ruff` заменяет `flake8`+`isort`+`pyflakes`+`pyupgrade`. Написан на Rust, работает в 10-100x быстрее.                                                           |
| 💻 Практика    | 25 мин | Установить: `uv add --dev ruff black mypy`. Создать `pyproject.toml`: `toml<br>[tool.ruff]<br>line-length = 88<br>select = ["E", "F", "W", "I", "N", "UP", "B", "SIM"]<br>[tool.ruff.isort]<br>known-first-party = ["myapp"]` |
| 🔍 Инструменты | 15 мин | **Terminal**: `uv run ruff check src/ --fix`. Увидеть: исправления за 0.02s. Проверить: `black` только форматирует, `ruff` линтит + форматирует (через `ruff format`).                                                        |
| 📝 Фиксация    | 5 мин  | Коммит: `feat(module3): configure ruff as primary linter/formatter`. Записать: `ruff format` совместим с `black`, используйте его вместо отдельного запуска.                                                                  |

📦 **Артефакт дня:** `pyproject.toml` секция `[tool.ruff]`, скриншот `ruff check --fix` времени.

### 📅 День 2: `mypy`/`pyright`, `strict` mode, type stubs

| Этап           | Время  | Действие                                                                                                                                                                  |
| -------------- | ------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 📖 Теория      | 15 мин | Разобрать: `mypy` = эталон, строгий, медленнее. `pyright` = быстрый, default в VS Code/Pylance. `strict = true` включает 7+ флагов. `types-requests` = stubs.             |
| 💻 Практика    | 25 млн | Добавить `pyproject.toml`: `toml<br>[tool.mypy]<br>strict = true<br>warn_unused_ignores = true<br>plugins = ["pydantic.mypy"]`<br>Запустить: `uv run mypy src/ --strict`. |
| 🔍 Инструменты | 15 мин | **Terminal**: увидеть ошибки типа `arg-type`, `no-untyped-def`. Проверить: `--strict` ловит 95% багов до runtime.                                                         |
| 📝 Фиксация    | 5 мин  | Коммит: `feat(module3): enable mypy strict mode`. Записать: `strict` ломает legacy, но спасает от `AttributeError`/`TypeError`.                                           |

📦 **Артефакт дня:** `[tool.mypy]` конфиг, скриншот `mypy` вывода.

### 📅 День 3: `pre-commit`, хуки, CI-интеграция

| Этап           | Время  | Действие                                                                                                                                                                                                                                                                                                                                                                                    |
| -------------- | ------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 📖 Теория      | 15 мин | Понять: `pre-commit` запускает линтеры до коммита. `.pre-commit-config.yaml` = декларативный конфиг. Хуки кэшируются, работают изолированно.                                                                                                                                                                                                                                                |
| 💻 Практика    | 25 млн | Создать `.pre-commit-config.yaml`: `yaml<br>repos:<br>  - repo: https://github.com/astral-sh/ruff-pre-commit<br>    rev: v0.4.8<br>    hooks: [{id: ruff, args: [--fix]}, {id: ruff-format}]<br>  - repo: https://github.com/pre-commit/mirrors-mypy<br>    rev: v1.10.0<br>    hooks: [{id: mypy, additional_dependencies: [pydantic]}]`<br>Инициализировать: `uv run pre-commit install`. |
| 🔍 Инструменты | 15 мин | **Terminal**: `git add . && git commit -m "test"`. Увидеть: ruff/mypy запускаются, коммит блокируется при ошибках.                                                                                                                                                                                                                                                                          |
| 📝 Фиксация    | 5 мин  | Коммит: `chore(module3): add pre-commit hooks for ruff & mypy`. Записать: `pre-commit` = первый слой защиты, не пропускайте `--no-verify`.                                                                                                                                                                                                                                                  |

📦 **Артефакт дня:** `.pre-commit-config.yaml`, скриншот блокировки коммита.

### 📅 День 4: Практика — Настроить `pyproject.toml` + pre-commit, прогнать на коде

| Этап           | Время  | Действие                                                                                                                                            |
| -------------- | ------ | --------------------------------------------------------------------------------------------------------------------------------------------------- |
| 📖 Теория      | 15 мин | Повторить чек-лист: 1) `ruff` + `mypy` в `pyproject.toml`. 2) `pre-commit` хуки. 3) `--strict`/`--fix`. 4) CI gate. 5) `ignore` правила для легаси. |
| 💻 Практика    | 25 мин | Взять `legacy_code.py`, запустить `pre-commit run --all-files`. Исправить 100% ошибок. Добавить `# noqa: F841` только где необходимо.               |
| 🔍 Инструменты | 15 мин | **VS Code**: включить `python.linting.enabled`. Увидеть: ошибки подсвечиваются в IDE синхронно с pre-commit.                                        |
| 📝 Фиксация    | 5 мин  | Коммит: `refactor(module3): fix all lint/type errors, enable CI gate`. Сохранить `audit/lint-fix-report.md`.                                        |

📦 **Артефакт дня:** Исправленный `legacy_code.py`, отчёт `pre-commit`.

### 📅 День 5: Инструменты: `pyright` vs `mypy`, `ruff` rules, `deptry`

| Этап           | Время  | Действие                                                                                                                                   |
| -------------- | ------ | ------------------------------------------------------------------------------------------------------------------------------------------ |
| 📖 Теория      | 15 мин | Разобрать: `pyright` быстрее, `mypy` строже. `ruff` кастомные правила через `tool.ruff.lint`. `deptry` + `ruff` = полный стек анализа.     |
| 💻 Практика    | 25 млн | Сравнить: `uv run pyright src/` vs `uv run mypy src/`. Увидеть: pyright <1s, mypy ~3s. Добавить в `ruff`: `ignore = ["E501", "D100"]`.     |
| 🔍 Инструменты | 15 мин | **Terminal**: `uv run ruff rule F401` → увидеть описание правила. Проверить: `ruff` поддерживает 1000+ правил.                             |
| 📝 Фиксация    | 5 мин  | Коммит: `chore(module3): configure pyright alternative & ruff ignore rules`. Записать: `mypy` = CI, `pyright` = IDE, `ruff` = lint+format. |

📦 **Артефакт дня:** Скриншот сравнения pyright/mypy времени.

### 📅 День 6: Итог недели — Документация и чек-лист

| Этап           | Время  | Действие                                                                                                                                                      |
| -------------- | ------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 📖 Теория      | 15 мин | Документация Code Style: `ruff`, `mypy`/`pyright`, `pre-commit`, `strict` mode, CI gates. Архитектурное решение: качество = автоматизация, не ревью-чеклисты. |
| 💻 Практика    | 25 мин | Создать `module3/README-code-style.md`. Собрать файлы. Прогн `pre-commit run --all-files`.                                                                    |
| 🔍 Инструменты | 15 мин | Финальная проверка: 0 lint/type ошибок, pre-commit проходит, CI gate настроен, форматирование консистентно.                                                   |
| 📝 Фиксация    | 5 мин  | Коммит: `docs(module3): add Code Style & Static Analysis guidelines`. PR: "Модуль 3 завершён".                                                                |

✅ **Критерий приёмки Модуля 3:** `ruff` заменяет flake8/black/isort, `mypy --strict` включён, `pre-commit` блокирует коммиты с ошибками, CI gate настроен, 100% легаси-ошибок исправлены, документация готова.

---

## 📘 Модуль 4 — Логирование и конфигурация

**Цель:** Настроить структурированные JSON-логи, валидацию `.env` через `pydantic-settings` и безопасное управление секретами.

### 📅 День 1: `logging` vs `structlog`, JSON-формат, уровни

| Этап           | Время  | Действие                                                                                                                                                                                                                       |
| -------------- | ------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| 📖 Теория      | 15 мин | Прочитать: [structlog docs](https://www.structlog.org/). Понять: `logging` = stdlib, verbose. `structlog` = структурированные ключи, JSON, pipeline processors.                                                                |
| 💻 Практика    | 25 мин | Создать `module4/logger_setup.py`: `python<br>import structlog<br>structlog.configure(processors=[structlog.processors.JSONRenderer()])<br>log = structlog.get_logger()<br>log.info("app_start", version="1.0.0", env="prod")` |
| 🔍 Инструменты | 15 мин | **Terminal**: `python logger_setup.py` → увидеть JSON: `{"event": "app_start", "version": "1.0.0", ...}`. Проверить: парсится Loki/ELK.                                                                                        |
| 📝 Фиксация    | 5 мин  | Коммит: `feat(module4): configure structlog JSON rendering`. Записать: `structlog` = pipeline, каждый processor трансформирует event.                                                                                          |

📦 **Артефакт дня:** `logger_setup.py`, скриншот JSON-вывода.

### 📅 День 2: `pydantic-settings`, `.env` парсинг, валидация

| Этап           | Время  | Действие                                                                                                                                                                                                                                                                                                           |
| -------------- | ------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| 📖 Теория      | 15 мин | Разобрать: `pydantic.BaseSettings` читает `.env`, `os.environ`, CLI args. Валидирует типы, бросает `ValidationError` при ошибках.                                                                                                                                                                                  |
| 💻 Практика    | 25 млн | Создать `settings.py`: `python<br>from pydantic_settings import BaseSettings, SettingsConfigDict<br>class Settings(BaseSettings):<br>    model_config = SettingsConfigDict(env_file=".env", env_file_encoding="utf-8")<br>    db_url: str<br>    debug: bool = False<br>    api_key: str<br>settings = Settings()` |
| 🔍 Инструменты | 15 мин | **Terminal**: создать `.env` с валидными/невалидными данными. Увидеть: `ValidationError` при missing `api_key`. Проверить: типы кастятся автоматически.                                                                                                                                                            |
| 📝 Фиксация    | 5 мин  | Коммит: `feat(module4): implement pydantic-settings config`. Записать: `Settings` = singleton, инициализируйте один раз при старте.                                                                                                                                                                                |

📦 **Артефакт дня:** `settings.py`, скриншот `ValidationError`.

### 📅 День 3: Секреты, `.env` hygiene, `os.environ` fallbacks

| Этап           | Время  | Действие                                                                                                                                                                                                                                                                                         |
| -------------- | ------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| 📖 Теория      | 15 мин | Понять: `.env` никогда не коммитится. `pydantic-settings` читает `.env`, но в prod = `os.environ` или Vault. `dotenv` = dev-only.                                                                                                                                                                |
| 💻 Практика    | 25 млн | Настроить `settings.py` с fallbacks: `python<br>class Settings(BaseSettings):<br>    model_config = SettingsConfigDict(env_prefix="APP_")<br>    db_url: str = "postgresql://localhost/db"<br>    api_key: str = os.getenv("API_KEY", "dev-key")`<br>Добавить `.env.example`, `.gitignore .env`. |
| 🔍 Инструменты | 15 мин | **Terminal**: `grep -r "password" src/` → 0 matches. Проверить: `git check-ignore .env` → true.                                                                                                                                                                                                  |
| 📝 Фиксация    | 5 мин  | Коммит: `chore(module4): secure secrets & add .env hygiene`. Записать: prod config = env vars only, `.env` = local dev convenience.                                                                                                                                                              |

📦 **Артефакт дня:** `.env.example`, `.gitignore`, скриншот `grep` проверки.

### 📅 День 4: Практика — Настроить JSON-логи, вынести конфиг в Settings класс

| Этап           | Время  | Действие                                                                                                                                                      |
| -------------- | ------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 📖 Теория      | 15 мин | Повторить чек-лист: 1) `structlog` JSON pipeline. 2) `pydantic-settings` валидация. 3) Correlation ID injection. 4) Log rotation (файлы).                     |
| 💻 Практика    | 25 мин | Собрать `app_logging.py`: объединить `structlog` + `settings` + `logging.handlers.RotatingFileHandler`. Добавить `request_id` в контекст через `contextvars`. |
| 🔍 Инструменты | 15 мин | **Vitest/pytest**: замокать `structlog`, проверить вызовы с `request_id`. Увидеть: логи пишутся в файл, JSON парсится.                                        |
| 📝 Фиксация    | 5 мин  | Коммит: `feat(module4): build production logging & config pipeline`. Сохранить `audit/logging-config-report.md`.                                              |

📦 **Артефакт дня:** `app_logging.py`, отчёт тестов.

### 📅 День 5: Инструменты: `rich` console, log correlation, `loguru` alternative

| Этап           | Время  | Действие                                                                                                                                                                           |
| -------------- | ------ | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 📖 Теория      | 15 мин | Разобрать: `rich` = красивая консольная отладка. `loguru` = упрощённый `logging`, но менее гибкий в prod. Correlation ID = `traceparent`/`x-request-id`.                           |
| 💻 Практика    | 25 млн | Добавить `rich` в dev: `python<br>if settings.debug: structlog.configure(processors=[rich_tracebacks, structlog.dev.ConsoleRenderer()])`<br>Проверить: dev = цветной, prod = JSON. |
| 🔍 Инструменты | 15 мин | **Terminal**: `python app_logging.py --debug` → увидеть цветной вывод. Проверить: prod log = чистый JSON, 0 ANSI-кодов.                                                            |
| 📝 Фиксация    | 5 мин  | Коммит: `chore(module4): add rich dev logging & correlation IDs`. Записать: `loguru` хорош для скриптов, `structlog` = enterprise стандарт.                                        |

📦 **Артефакт дня:** Скриншот `rich` dev-лога vs prod JSON.

### 📅 День 6: Итог недели — Документация и чек-лист

| Этап           | Время  | Действие                                                                                                                                                                                                                 |
| -------------- | ------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| 📖 Теория      | 15 мин | Документация Logging/Config: `structlog`, `pydantic-settings`, `.env` hygiene, correlation IDs, `rich` dev, log rotation. Архитектурное решение: конфигурация = валидированный объект, логи = структурированные события. |
| 💻 Практика    | 25 мин | Создать `module4/README-logging-config.md`. Собрать файлы. Прогн JSON-парсер на логах.                                                                                                                                   |
| 🔍 Инструменты | 15 мин | Финальная проверка: `.env` валидируется, JSON-логи пишутся, correlation ID propagates, `rich` only in dev, 0 secrets leaked.                                                                                             |
| 📝 Фиксация    | 5 мин  | Коммит: `docs(module4): add Logging & Configuration guidelines`. PR: "Модуль 4 завершён".                                                                                                                                |

✅ **Критерий приёмки Модуля 4:** `structlog` генерирует JSON, `pydantic-settings` валидирует `.env`, correlation ID внедрён, `.env` не коммитится, dev/prod логи разделены, документация готова.

---

## 📘 Модуль 5 — CLI и скриптинг

**Цель:** Создавать CLI-утилиты с подкомандами через `typer`/`click`, настраивать `__main__.py`, entry points и упаковку в `pip install .`.

### 📅 День 1: `argparse` vs `typer`/`click`, type hints → CLI args

| Этап           | Время  | Действие                                                                                                                                                                                                                                                                 |
| -------------- | ------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| 📖 Теория      | 15 мин | Прочитать: [Typer Docs](https://typer.tiangolo.com/). Понять: `typer` использует type hints для генерации CLI, валидации и `--help`. `argparse` = stdlib, verbose, ручной парсинг.                                                                                       |
| 💻 Практика    | 25 мин | Создать `module5/cli_basic.py`: `python<br>import typer<br>app = typer.Typer()<br>@app.command()<br>def greet(name: str, verbose: bool = False):<br>    typer.echo(f"Hello {name}!" if not verbose else f"[DEBUG] Greeting {name}")<br>if __name__ == "__main__": app()` |
| 🔍 Инструменты | 15 мин | **Terminal**: `python cli_basic.py greet Dev --verbose`. Проверить: `--help` генерируется автоматически. Type hints → CLI args.                                                                                                                                          |
| 📝 Фиксация    | 5 мин  | Коммит: `feat(module5): scaffold Typer CLI app`. Записать: `typer` = `click` + type hints, 80% boilerplate removed.                                                                                                                                                      |

📦 **Артефакт дня:** `cli_basic.py`, скриншот `--help` вывода.

### 📅 День 2: Подкоманды, группы, `typer.Option`/`typer.Argument`

| Этап           | Время  | Действие                                                                                                                                                                                   |
| -------------- | ------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| 📖 Теория      | 15 мин | Разобрать: `app.add_typer(sub_app)` = subcommands. `Option` = `--flag`, `Argument` = позиционный. `rich` интеграция для прогресс-баров.                                                    |
| 💻 Практика    | 25 млн | Добавить подкоманды: `python<br>db = typer.Typer()<br>@db.command()<br>def migrate(path: str): ...<br>app.add_typer(db, name="db")`<br>Запустить: `python cli.py db migrate ./migrations`. |
| 🔍 Инструменты | 15 мин | **Terminal**: `python cli.py db --help` → увидеть подкоманды. Проверить: `typer.Argument` vs `typer.Option` поведение.                                                                     |
| 📝 Фиксация    | 5 мин  | Коммит: `feat(module5): add CLI subcommands & groups`. Записать: подкоманды = изоляция логики, не монолитный `argparse`.                                                                   |

📦 **Артефакт дня:** Обновлённый `cli.py`, скриншот `db --help`.

### 📅 День 3: `__main__.py`, `entry_points`, `pip install .`

| Этап           | Время  | Действие                                                                                                                                                                                                                        |
| -------------- | ------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 📖 Теория      | 15 мин | Понять: `__main__.py` = `python -m mycli`. `project.scripts` в `pyproject.toml` = `mycli` глобальная команда. `build`/`twine` для wheel.                                                                                        |
| 💻 Практика    | 25 млн | Настроить `pyproject.toml`: `toml<br>[project.scripts]<br>mycli = "myapp.cli:app"<br>[build-system]<br>requires = ["hatchling"]<br>build-backend = "hatchling.build"`<br>Запустить: `uv run pip install -e .` → `mycli --help`. |
| 🔍 Инструменты | 15 мин | **Terminal**: `which mycli` → путь в `.venv/bin/`. Проверить: `python -m myapp.cli` тоже работает.                                                                                                                              |
| 📝 Фиксация    | 5 мин  | Коммит: `chore(module5): add entry_points & editable install`. Записать: `pip install -e .` = dev mode, `build` = prod wheel.                                                                                                   |

📦 **Артефакт дня:** `pyproject.toml` секция `[project.scripts]`, скриншот `mycli` запуска.

### 📅 День 4: Практика — Создать CLI-утилиту с подкомандами, собрать в pip install

| Этап           | Время  | Действие                                                                                                                                                          |
| -------------- | ------ | ----------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 📖 Теория      | 15 мин | Повторить чек-лист: 1) Typer app. 2) Подкоманды. 3) Type validation. 4) `__main__.py`. 5) `entry_points`. 6) `uv build`.                                          |
| 💻 Практика    | 25 мин | Собрать `full_cli/`: добавить `init`, `run`, `clean` команды. Настроить `hatch build`. Запустить `uv build` → `dist/*.whl`. Установить: `pip install dist/*.whl`. |
| 🔍 Инструменты | 15 мин | **Terminal**: `mycli init --name test` → увидеть вывод. Проверить: `pip show mycli` → version, location.                                                          |
| 📝 Фиксация    | 5 мин  | Коммит: `feat(module5): build & package CLI utility`. Сохранить `audit/cli-packaging-report.md`.                                                                  |

📦 **Артефакт дня:** `dist/*.whl`, скриншот `mycli` работы.

### 📅 День 5: Инструменты: Shell completion, rich progress bars, `argparse` fallback

| Этап           | Время  | Действие                                                                                                                                                       |
| -------------- | ------ | -------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 📖 Теория      | 15 мин | Разобрать: `typer` генерирует `bash`/`zsh`/`fish` completion. `typer.progress()` = rich progress bars. `argparse` = legacy, но иногда нужен для совместимости. |
| 💻 Практика    | 25 млн | Добавить completion: `mycli --install-completion`. Добавить прогресс: `python<br>with typer.progressbar(tasks) as progress: for t in progress: process(t)`     |
| 🔍 Инструменты | 15 мин | **Terminal**: `source ~/.bash_completion.d/mycli` → `mycli <TAB>` → автодополнение. Проверить: прогресс-бар работает.                                          |
| 📝 Фиксация    | 5 мин  | Коммит: `chore(module5): add shell completion & progress bars`. Записать: completion = UX boost, `argparse` только для legacy-интеграций.                      |

📦 **Артефакт дня:** Скриншот shell completion и rich progress bar.

### 📅 День 6: Итог недели — Документация и чек-лист

| Этап           | Время  | Действие                                                                                                                                                                             |
| -------------- | ------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| 📖 Теория      | 15 мин | Документация CLI: `typer`, subcommands, `entry_points`, `__main__.py`, packaging, shell completion. Архитектурное решение: CLI = type hints + declarative config, не ручной парсинг. |
| 💻 Практика    | 25 мин | Создать `module5/README-cli-scripting.md`. Собрать файлы. Прогн `uv build` + `pip install`.                                                                                          |
| 🔍 Инструменты | 15 мин | Финальная проверка: подкоманды работают, completion активен, wheel собирается, `mycli` глобально доступен.                                                                           |
| 📝 Фиксация    | 5 мин  | Коммит: `docs(module5): add CLI & Scripting guidelines`. PR: "Модуль 5 завершён".                                                                                                    |

✅ **Критерий приёмки Модуля 5:** CLI-утилита построена на `typer`, подкоманды разделены, `entry_points` настроены, `pip install .` работает, shell completion активен, progress bars работают, документация готова.

---

## 📘 Модуль 6 — Обработка ошибок и retry

**Цель:** Реализовать отказоустойчивость через кастомные исключения, `contextlib`, `tenacity` retry-паттерны и exponential backoff.

### 📅 День 1: Exception hierarchy, custom exceptions, `raise from`

| Этап           | Время  | Действие                                                                                                                                                                                                                                       |
| -------------- | ------ | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 📖 Теория      | 15 мин | Прочитать: [Python Exceptions](https://docs.python.org/3/tutorial/errors.html). Понять: наследуйте от `Exception`, не `BaseException`. `raise NewError() from old` сохраняет цепочку.                                                          |
| 💻 Практика    | 25 мин | Создать `module4/exceptions.py`: `python<br>class AppError(Exception): pass<br>class NetworkError(AppError): pass<br>class TimeoutError(NetworkError): pass<br>try: ... except requests.Timeout as e: raise TimeoutError("DB timeout") from e` |
| 🔍 Инструменты | 15 мин | **Terminal**: запустить, увидеть `Traceback` с `The above exception was the direct cause of...`. Проверить: `__cause__` сохраняет оригинал.                                                                                                    |
| 📝 Фиксация    | 5 мин  | Коммит: `feat(module6): define custom exception hierarchy & raise from`. Записать: `raise from` = стандарт для tracing, не теряйте стек.                                                                                                       |

📦 **Артефакт дня:** `exceptions.py`, скриншот `Traceback` с цепочкой.

### 📅 День 2: `contextlib`, `@contextmanager`, `AsyncExitStack`

| Этап           | Время  | Действие                                                                                                                                                                                                                     |
| -------------- | ------ | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 📖 Теория      | 15 мин | Разобрать: `contextlib.contextmanager` = generator-based context. `yield` = enter, `finally` = exit. `AsyncExitStack` = динамическое управление ресурсами.                                                                   |
| 💻 Практика    | 25 млн | Создать `context_tools.py`: `python<br>@contextmanager<br>def temporary_file(suffix=".txt"):<br>    f = tempfile.NamedTemporaryFile(delete=False, suffix=suffix)<br>    try: yield f.name<br>    finally: os.unlink(f.name)` |
| 🔍 Инструменты | 15 мин | **Terminal**: `with temporary_file() as path: ...`. Проверить: файл удаляется после блока, даже при ошибке.                                                                                                                  |
| 📝 Фиксация    | 5 мин  | Коммит: `feat(module6): implement contextlib context managers`. Записать: `contextmanager` заменяет `try/finally` boilerplate.                                                                                               |

📦 **Артефакт дня:** `context_tools.py`, скриншот временного файла.

### 📅 День 3: `tenacity` library, retry decorators, backoff strategies

| Этап           | Время  | Действие                                                                                                                                                                                                                                                                                                       |
| -------------- | ------ | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 📖 Теория      | 15 мин | Понять: `tenacity` = declarative retries. `@retry(stop=..., wait=..., retry=...)`. Exponential backoff = `wait_exponential()`. `reraise=True` сохраняет exception.                                                                                                                                             |
| 💻 Практика    | 25 млн | Установить: `uv add tenacity`. Добавить: `python<br>from tenacity import retry, stop_after_attempt, wait_exponential, retry_if_exception_type<br>@retry(stop=stop_after_attempt(3), wait=wait_exponential(multiplier=1, min=2, max=10), retry=retry_if_exception_type(NetworkError))<br>def fetch_data(): ...` |
| 🔍 Инструменты | 15 мин | **Terminal**: имитировать ошибку → увидеть 3 попытки с задержкой 2s, 4s, 8s. Проверить: `reraise=True` бросает `NetworkError` после 3-й попытки.                                                                                                                                                               |
| 📝 Фиксация    | 5 мин  | Коммит: `feat(module6): add tenacity retry with exponential backoff`. Записать: `tenacity` = threadsafe, async-compatible, production-ready.                                                                                                                                                                   |

📦 **Артефакт дня:** Обновлённый `fetch_data.py`, скриншот retry логики.

### 📅 День 4: Практика — Реализовать retry с exponential backoff для HTTP-вызовов

| Этап           | Время  | Действие                                                                                                                                                                                                                                                                                                                                                                       |
| -------------- | ------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| 📖 Теория      | 15 мин | Повторить чек-лист: 1) Custom exceptions. 2) `@retry` decorator. 3) `Retry-After` header handling. 4) `after` callback для logging. 5) Idempotency guard.                                                                                                                                                                                                                      |
| 💻 Практика    | 25 мин | Собрать `http_client.py`: `python<br>@retry(wait=wait_exponential(), stop=stop_after_attempt(5), retry=retry_if_exception_type((TimeoutError, HTTPError)), after=after_log)<br>async def call_api(url: str):<br>    resp = await httpx.AsyncClient().get(url)<br>    if resp.status_code == 429: raise RetryAfterError(resp.headers["Retry-After"])<br>    return resp.json()` |
| 🔍 Инструменты | 15 мин | **Vitest/pytest**: мокнуть 429/503 → увидеть backoff + respect `Retry-After`. Проверить: GET ретраится, POST не ретраится без идемпотентности.                                                                                                                                                                                                                                 |
| 📝 Фиксация    | 5 мин  | Коммит: `feat(module6): build resilient HTTP client with tenacity`. Сохранить `audit/http-retry-test.md`.                                                                                                                                                                                                                                                                      |

📦 **Артефакт дня:** `http_client.py`, отчёт тестов.

### 📅 День 5: Инструменты: `traceback.format_exc()`, `sys.excepthook`, circuit breaker basics

| Этап           | Время  | Дейстье                                                                                                                                                                                                                         |
| -------------- | ------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 📖 Теория      | 15 мин | Разобрать: `traceback` = форматирование стека. `sys.excepthook` = global unhandled handler. Circuit breaker = open/half-open/closed states для предотвращения cascade failure.                                                  |
| 💻 Практика    | 25 млн | Добавить global hook: `python<br>def global_handler(exc_type, exc_value, exc_tb): log.critical("Uncaught", exc_info=(exc_type, exc_value, exc_tb))<br>sys.excepthook = global_handler`<br>Настроить `circuitbreaker` декоратор. |
| 🔍 Инструменты | 15 мин | **Terminal**: вызвать необработанную ошибку → увидеть JSON-лог. Проверить: circuit breaker открывает цепь после N failures.                                                                                                     |
| 📝 Фиксация    | 5 мин  | Коммит: `chore(module6): add global exception hook & circuit breaker`. Записать: `sys.excepthook` = последний рубеж, логируйте всегда.                                                                                          |

📦 **Артефакт дня:** `global_handler.py`, скриншот uncaught JSON-лога.

### 📅 День 6: Итог недели — Документация и чек-лист

| Этап           | Время  | Действие                                                                                                                                                                                                                      |
| -------------- | ------ | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 📖 Теория      | 15 мин | Документация Error Handling: custom exceptions, `raise from`, `contextlib`, `tenacity`, retry backoff, `Retry-After`, circuit breaker, global hooks. Архитектурное решение: ошибки = управляемый поток, не хаотичные падения. |
| 💻 Практика    | 25 мин | Создать `module6/README-error-handling.md`. Собрать файлы. Прогн retry-тесты.                                                                                                                                                 |
| 🔍 Инструменты | 15 мин | Финальная проверка: retry работает, backoff exponential, `Retry-After` уважается, circuit breaker открывается, логи структурированы.                                                                                          |
| 📝 Фиксация    | 5 мин  | Коммит: `docs(module6): add Error Handling & Retry guidelines`. PR: "Модуль 6 завершён".                                                                                                                                      |

✅ **Критерий приёмки Модуля 6:** Кастомные исключения иерархичны, `raise from` сохраняет стек, `contextlib` управляет ресурсами, `tenacity` retry с exponential backoff работает, `Retry-After` учитывается, circuit breaker настроен, документация готова.

---

## 🛠️ Готовые команды для быстрого старта Фазы 1 (Python) (PowerShell)

```powershell
# 1. Создание структуры модулей
mkdir python-fundamentals/phase1-basics && cd $_
mkdir module1-env, module2-syntax, module3-style, module4-config, module5-cli, module6-errors

# 2. Установка uv и Python
curl -LsSf https://astral.sh/uv/install.ps1 | iex
uv python install 3.12
uv python pin 3.12

# 3. Инициализация проекта и зависимостей
cd module1-env && uv init . && uv add requests pydantic structlog tenacity typer
uv sync --frozen-lockfile

# 4. Запуск линтинга и типизации
uv add --dev ruff mypy pre-commit
uv run ruff check . --fix
uv run mypy src/ --strict

# 5. Запуск CLI-утилиты
cd ../module5-cli
uv run python cli.py greet World --verbose
uv build
pip install dist/*.whl

# 6. Тестирование retry-логики
cd ../module6-errors
uv run python http_client.py
# Увидеть: retry attempts with exponential backoff
```

---

## 💡 Рекомендации преподавателя на эту фазу

1. **`uv` заменяет `venv`/`pip`/`pip-tools`**. Используйте `uv sync --frozen-lockfile` в CI. `uv.lock` = детерминизм. `deptry` ловит unused imports.
2. **Python 3.10+ = `match`, dataclasses, walrus**. Не пишите `if/elif` цепочки, используйте `match/case`. `frozen=True` = immutable by default.
3. **`ruff` + `mypy` = production baseline**. `ruff` линтит/формирует в миллисекундах. `mypy --strict` ловит 90% багов до runtime. `pre-commit` блокирует коммиты.
4. **Конфигурация = `pydantic-settings`, логи = `structlog` JSON**. `.env` только для dev. `pydantic` валидирует типы при старте. `structlog` pipeline = структурированные события.
5. **2026 reality:** `uv` стал де-факто стандартом для Python packaging. `typer` вытесняет `argparse`/`click`. `tenacity` = стандартный retry. `pydantic-settings` заменяет `python-dotenv` + ручной парсинг. `ruff` заменяет 4+ линтера.

---
