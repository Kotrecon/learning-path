# ✅ Checklist: Модуль 1 — Среда и управление зависимостями

## 🎯 Критерии приёмки модуля

Перед тем как создать PR `"Модуль 1 завершён"`, убедись, что ВСЕ пункты выполнены:

### 📦 Проект и окружение

- [ ] Проект создан через `uv init my-python-project`
- [ ] В корне проекта есть `.python-version` с версией `3.12` (или `3.12.4`)
- [ ] `.venv/` НЕ коммитится (есть в `.gitignore`)
- [ ] `uv.lock` СМОТРИТСЯ в Git (коммитится)

### 📄 pyproject.toml

- [ ] `[project]` секция содержит: `name`, `version`, `description`, `requires-python`
- [ ] `dependencies` содержит минимум: `requests`, `pydantic`, `structlog`
- [ ] `[dependency-groups] dev` содержит: `pytest`, `ruff`, `mypy`, `deptry`
- [ ] Файл соответствует PEP 621 (нет `setup.py`, нет `requirements.txt` как основного источника)

### 🔒 Lockfile и детерминизм

- [ ] `uv.lock` сгенерирован автоматически (не редактировался вручную)
- [ ] `uv sync --frozen-lockfile` работает без ошибок
- [ ] При изменении `pyproject.toml` → `uv lock` обновляет `uv.lock`
- [ ] При удалении `uv.lock` → `uv sync` пересоздаёт его детерминированно

### 🧪 Dev-зависимости и инструменты

- [ ] `uv add --dev pytest ruff mypy deptry` выполнено
- [ ] `uv pip list` показывает prod + dev пакеты
- [ ] `uv run deptry .` не находит unused/missing imports (или исправлены)
- [ ] `uv run ruff check . --fix` проходит (0 ошибок)
- [ ] `uv run mypy src/ --strict` проходит (0 ошибок)

### 📝 Документация

- [ ] Создан `README-env-deps.md` с описанием:
  - Как настроить окружение (`uv init`, `uv sync`)
  - Как добавлять зависимости (`uv add`, `uv add --dev`)
  - Как работать с lockfile (`--frozen-lockfile` в CI)
  - Как запускать инструменты (`ruff`, `mypy`, `deptry`)
- [ ] В `PROGRESS.md` записаны все 6 дней (дата, тема, время, проблема, решение, завтра)

### 🔄 Git и workflow

- [ ] Создана ветка: `python-phase01-mod01`
- [ ] 6 коммитов (по одному на день) с понятными сообщениями:
  - `feat(module1): init project with uv & pyproject.toml`
  - `chore(module1): enforce frozen lockfile in workflow`
  - `chore(module1): add requirements.txt export for legacy CI`
  - `feat(module1): scaffold production-ready uv project`
  - `chore(module1): add deptry & python version pin`
  - `docs(module1): add Environment & Dependencies guidelines`
- [ ] Создан PR с заголовком `"Модуль 1 завершён"`
- [ ] В описании PR — краткое резюме: что сделано, какие проблемы были, как решил

---

## 🚀 После завершения модуля

1. **После ревью** → мердж в `main`
2. **Удалить ветку**: `git branch -d python-phase01-mod01`
3. **Удалить удалённую ветку**: `git push origin --delete python-phase01-mod01`
4. **Записать инсайт** в `languages/python/PROGRESS.md` → раздел "Инсайты и заметки"
5. **Начать Модуль 2**: создать ветку `python-phase01-mod02`

---

## 📌 Если что-то пошло не так

| Проблема                           | Решение                                                                 |
| ---------------------------------- | ----------------------------------------------------------------------- |
| `uv sync --frozen-lockfile` падает | `uv lock` → обновить lockfile → коммит                                  |
| `deptry` находит unused imports    | Удалить неиспользуемый импорт ИЛИ `uv add` недостающий пакет            |
| `mypy` находит ошибки типа         | Добавить типизацию или `# type: ignore` (только если оправдано)         |
| `.venv/` коммитится в Git          | Добавить `.venv/` в `.gitignore` → `git rm -r --cached .venv/` → коммит |
| `uv` не установлен                 | `curl -LsSf https://astral.sh/uv/install.ps1 \| iex` (PowerShell)       |

---

## 🎯 Ссылки

- [theory.md](theory.md) — теория модуля
- [task.md](task.md) — практика по дням
- [README.md](../../README.md) — план Фазы 1
- [PROGRESS.md](../../../PROGRESS.md) — таблица прогресса
