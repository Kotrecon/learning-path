# 🛠️ Локальная проверка проекта (Python)
# Запуск: из корня learning-path/
#          cd learning-path
#          .\scripts\check.ps1

.param(
    [string]$Module = "languages/python/phase-01-foundation/module-01-env-deps"
)

Write-Host "🔍 Запуск проверки проекта..." -ForegroundColor Cyan
Write-Host "📁 Модуль: $Module`n" -ForegroundColor Yellow

# Перейти в папку модуля
Set-Location $Module

# 1. Проверка: uv установлен?
Write-Host "✅ Шаг 1: Проверка uv" -ForegroundColor Yellow
uv --version
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ uv не установлен! Установи: curl -LsSf https://astral.sh/uv/install.ps1 | iex" -ForegroundColor Red
    exit 1
}

# 2. Проверка: pyproject.toml существует?
Write-Host "`n✅ Шаг 2: Проверка pyproject.toml" -ForegroundColor Yellow
if (-not (Test-Path "pyproject.toml")) {
    Write-Host "❌ pyproject.toml не найден! Выполни: uv init" -ForegroundColor Red
    exit 1
}

# 3. Проверка: uv.lock существует?
Write-Host "`n✅ Шаг 3: Проверка uv.lock" -ForegroundColor Yellow
if (-not (Test-Path "uv.lock")) {
    Write-Host "❌ uv.lock не найден! Выполни: uv add <package>" -ForegroundColor Red
    exit 1
}

# 4. Синхронизация с lockfile
Write-Host "`n✅ Шаг 4: uv sync --frozen-lockfile" -ForegroundColor Yellow
uv sync --frozen-lockfile
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ uv sync failed! Устарел lockfile? Выполни: uv lock" -ForegroundColor Red
    exit 1
}

# 5. Проверка зависимостей (ruff)
Write-Host "`n✅ Шаг 5: ruff check . --fix" -ForegroundColor Yellow
uv run ruff check . --fix
if ($LASTEXITCODE -ne 0) {
    Write-Host "⚠️ ruff нашёл ошибки. Исправь вручную или выполни: uv run ruff check . --fix" -ForegroundColor Yellow
}

# 6. Проверка типизации (mypy)
Write-Host "`n✅ Шаг 6: mypy src/ --strict" -ForegroundColor Yellow
if (Test-Path "src") {
    uv run mypy src/ --strict
    if ($LASTEXITCODE -ne 0) {
        Write-Host "⚠️ mypy нашёл ошибки типа. Исправь или добавь # type: ignore" -ForegroundColor Yellow
    }
} else {
    Write-Host "⚠️ папка src/ не найдена. Пропускаю mypy." -ForegroundColor Yellow
}

# 7. Проверка unused imports (deptry)
Write-Host "`n✅ Шаг 7: deptry ." -ForegroundColor Yellow
uv run deptry .
if ($LASTEXITCODE -ne 0) {
    Write-Host "⚠️ deptry нашёл unused/missing imports. Исправь." -ForegroundColor Yellow
}

# 8. Граф зависимостей
Write-Host "`n✅ Шаг 8: uv pip tree" -ForegroundColor Yellow
uv pip tree

# Вернуться в корень
Set-Location ..\..\..\..

Write-Host "`n🎉 Проверка завершена!" -ForegroundColor Green
Write-Host "Если есть ⚠️ — исправь перед коммитом." -ForegroundColor Yellow
Write-Host "Если всё ✅ — можешь коммитить: git add . && git commit -m '...'" -ForegroundColor Cyan