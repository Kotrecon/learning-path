# 🐍 Python (Backend, Production). Полная дорожная карта обучения (35+ модулей)

> 💡 `Python` — это `универсальный, динамически типизированный язык` с мощной экосистемой для веба, данных, автоматизации и AI. В production-контексте это `не "скриптовый язык", а зрелая платформа` с строгой типизацией (Pydantic, mypy), асинхронным I/O (asyncio), современным ORM (SQLAlchemy 2.0), быстрыми фреймворками (FastAPI) и готовыми инструментами наблюдаемости. Ключ к мастерству: `понимать рантайм (CPython, GIL, GC)`, `контролировать async/event loop`, `писать типизированный, тестируемый код` и `готовить приложения к высоконагруженному деплою`.

## 📋 Оглавление

- [Основная мысль](#-основная-мысль)
- [Проблема](#️-проблема)
- [Структура обучения: 7 фаз](#️-структура-обучения-7-фаз)
- [Практические примеры кода (Python 3.11+)](#-практические-примеры-кода-python-311)
- [Production-чек-лист](#-production-чек-лист)
- [Типичные ошибки и антипаттерны](#-типичные-ошибки-и-антипаттерны)
- [Ресурсы и ссылки](#-ресурсы-и-ссылки)
- [Заключение + план действий](#-заключение--план-действий)

---

## 💡 Основная мысль

`Изучение production Python` решает проблему `"скриптового менталитета"`: вместо `requests` + глобальных переменных + `except:` вы получаете `типизированный, асинхронный, тестируемый стек` с предсказуемым поведением под нагрузкой. Архитектура обучения строится на `шести принципах`: (1) `Runtime-first` (GIL, GC, event loop, async/sync границы), (2) `Type-safety by default` (Pydantic v2, mypy/pyright, `typing`), (3) `Async I/O everywhere` (httpx, asyncpg, asyncio), (4) `Explicit architecture` (DI, слои, boundaries, lifespan), (5) `Observability-ready` (OpenTelemetry, structlog, health checks), (6) `Testable & deployable` (pytest, testcontainers, Docker, CI/CD). Компромисс: `требует дисциплины в типизации и async` окупается `скоростью разработки`, `предсказуемостью` и `готовностью к high-load`.

---

## ⚠️ Проблема

| Сценарий                                        | Без production-подхода в Python                                 | Последствие                                          |
| ----------------------------------------------- | --------------------------------------------------------------- | ---------------------------------------------------- |
| `requests` + синхронные вызовы в веб-сервере    | Блокировка event loop, thread pool starvation, 503 при нагрузке | 🔥 Падение под 100+ RPS, высокие задержки            |
| Глобальные состояния / синглтоны БД             | Гонки данных, кросс-запросные сессии, утечки соединений         | ❌ Непредсказуемые баги, повреждение данных          |
| `except:` без контекста, `print()` вместо логов | Невозможно дебажить в проде, теряются стектрейсы                | ⚠️ Долгое восстановление, слепая эксплуатация        |
| Pydantic v1 / SQLAlchemy 1.x стиль              | Устаревшие API, падение производительности, конфликт с v2       | 💥 Технические долги, сложная миграция               |
| Игнорирование типизации / mypy                  | Runtime-ошибки в продакшене, рефакторинг = страх                | 🚨 Падение качества, рост багов, замедление доставки |

**Без инженерного подхода**: Python становится «быстрым прототипом», который невозможно масштабировать, отлаживать или безопасно деплоить.

---

## 🗂️ Структура обучения: 7 фаз

### 📘 Фаза 1: Фундамент и современный тулинг (Модули 1-6)

| №   | Модуль                               | Цель                                                   | Ключевые навыки                                                | Практика                                                        |
| --- | ------------------------------------ | ------------------------------------------------------ | -------------------------------------------------------------- | --------------------------------------------------------------- |
| 1   | `Среда и управление зависимостями`   | Понимать изоляцию, `uv`/`poetry`, `pyproject.toml`     | Virtual env, lock-файлы, `uv sync`, `pip-tools`                | Создать проект с `uv`, добавить зависимости, сгенерировать lock |
| 2   | `Современный синтаксис Python 3.10+` | Использовать `match/case`, walrus, dataclasses, typing | Pattern matching, `:=`, `@dataclass(frozen=True)`, `TypeAlias` | Переписать классы на dataclasses, добавить `match` для парсинга |
| 3   | `Код-стайл и статический анализ`     | Автоматизировать форматирование, линтинг, типизацию    | `ruff`, `black`, `mypy`/`pyright`, `pre-commit`                | Настроить `pyproject.toml` + `pre-commit`, прогнать на коде     |
| 4   | `Логирование и конфигурация`         | Структурированные логи, env-конфиги, secrets           | `logging`, `structlog`, `pydantic-settings`, `.env`            | Настроить JSON-логи, вынести конфиг в `Settings` класс          |
| 5   | `CLI и скриптинг`                    | Создавать утилиты, парсить аргументы, entry points     | `click`/`typer`, `argparse`, `__main__.py`, packaging          | Создать CLI-утилиту с подкомандами, собрать в `pip install .`   |
| 6   | `Обработка ошибок и retry`           | Контекст, кастомные исключения, retry-паттерны         | `contextlib`, `traceback`, `tenacity`, retry policies          | Реализовать retry с exponential backoff для HTTP-вызовов        |

### 🔍 Фаза 2: Рантайм и глубокое понимание (Модули 7-12)

| №   | Модуль                                  | Цель                                                | Ключевые навыки                                       | Практика                                              |
| --- | --------------------------------------- | --------------------------------------------------- | ----------------------------------------------------- | ----------------------------------------------------- |
| 7   | `Управление памятью и GC`               | Понимать refcount, generational GC, weakref         | `sys.getrefcount`, `gc`, `weakref`, memory profiling  | Найти утечку памяти через `tracemalloc`, исправить    |
| 8   | `Итераторы, генераторы и асинхронность` | Экономить память, ленивые вычисления, `async for`   | `yield`, `itertools`, `async_generator`, `__aiter__`  | Реализовать ленивый парсер большого файла             |
| 9   | `Дескрипторы, свойства и мета-классы`   | Понимать, как работают ORM/Pydantic под капотом     | `__get__`/`__set__`, `@property`, `type.__new__`      | Написать кастомный дескриптор для валидации полей     |
| 10  | `Контекстные менеджеры`                 | Безопасное управление ресурсами (файлы, соединения) | `with`, `contextlib.asynccontextmanager`, `ExitStack` | Написать менеджер для временных файлов/БД-соединений  |
| 11  | `Функциональные паттерны`               | Чистые функции, иммутабельность, композиция         | `functools`, `operator`, `toolz`, pipes, currying     | Рефакторить цикл в функциональный pipeline            |
| 12  | `CPython internals & оптимизация`       | Bytecode, `dis`, профилирование, C-расширения       | `cProfile`, `line_profiler`, Cython, Numba, GIL       | Оптимизировать hot-path, сравнить `timeit` результаты |

### ⚡ Фаза 3: Конкурентность, Async и I/O (Модули 13-17)

| №   | Модуль                                    | Цель                                               | Ключевые навыки                                           | Практика                                                    |
| --- | ----------------------------------------- | -------------------------------------------------- | --------------------------------------------------------- | ----------------------------------------------------------- |
| 13  | `Threading vs Multiprocessing vs Asyncio` | Выбирать модель под задачу (CPU vs I/O bound)      | `concurrent.futures`, GIL, process pools, event loop      | Написать параллельный загрузчик изображений (async vs pool) |
| 14  | `asyncio Deep Dive`                       | Понимать event loop, tasks, futures, cancellation  | `asyncio.create_task`, `gather`, `wait_for`, `shield`     | Реализовать конкурентный парсер с таймаутами и отменой      |
| 15  | `Async I/O Patterns`                      | Неблокирующие HTTP, БД, файлы, connection pooling  | `httpx`, `asyncpg`, `aiofiles`, `async_lru`, backpressure | Написать async-клиент к API с retry и pooling               |
| 16  | `Очереди и Producer-Consumer`             | Балансировка нагрузки, rate limiting, backpressure | `asyncio.Queue`, `anyio`, channels, semaphores            | Реализовать очередь задач с ограничением параллелизма       |
| 17  | `Фоновые задачи и планировщики`           | Cron vs in-process, очереди, graceful shutdown     | Celery/ARQ, APScheduler, lifespan, `asyncio.to_thread`    | Создать фоновый процесс очистки старых записей              |

### 🌐 Фаза 4: Web API и фреймворки (Модули 18-23)

| №   | Модуль                           | Цель                                                     | Ключевые навыки                                            | Практика                                                     |
| --- | -------------------------------- | -------------------------------------------------------- | ---------------------------------------------------------- | ------------------------------------------------------------ |
| 18  | `HTTP-протокол и REST/GraphQL`   | Методы, статусы, идемпотентность, заголовки, кэширование | `2xx/4xx/5xx`, `ETag`, `Idempotency-Key`, OpenAPI          | Спроектировать REST API коллекции с правильными статусами    |
| 19  | `FastAPI архитектура`            | Routing, DI, middleware, lifespan, dependency overrides  | `FastAPI`, `Depends`, `lifespan`, `app.state`              | Создать приложение с DI-контейнером и graceful shutdown      |
| 20  | `Request/Response Handling`      | Валидация, сериализация, error handlers, problem details | `pydantic.BaseModel`, `HTTPException`, `APIRouter`         | Добавить кастомный обработчик ошибок RFC 7807                |
| 21  | `Authentication & Authorization` | JWT, OAuth2, API keys, role/policy-based access          | `python-jose`, `passlib`, middleware, security scopes      | Реализовать JWT-auth + policy-based access control           |
| 22  | `Middleware & Interceptors`      | CORS, rate limiting, request logging, compression        | `BaseHTTPMiddleware`, `slowapi`, `gzip`, custom middleware | Настроить CORS, rate limit 100 req/min, логирование запросов |
| 23  | `Real-time & WebSockets`         | WebSocket API, Server-Sent Events, Pub/Sub               | `WebSocket`, `ServerSentEvent`, Redis Pub/Sub              | Реализовать SSE-поток уведомлений                            |

### 💾 Фаза 5: Данные, ORM и валидация (Модули 24-27)

| №   | Модуль                        | Цель                                                    | Ключевые навыки                                                    | Практика                                                       |
| --- | ----------------------------- | ------------------------------------------------------- | ------------------------------------------------------------------ | -------------------------------------------------------------- |
| 24  | `SQLAlchemy 2.0`              | Core vs ORM, 2.0 style, async sessions, relationships   | `AsyncSession`, `select`, `relationship`, `lazy`/`selectin`        | Настроить async ORM, реализовать CRUD с правильными загрузками |
| 25  | `Alembic & миграции`          | Auto-migrations, data migrations, zero-downtime         | `alembic`, `--autogenerate`, expand/contract, rollback             | Создать миграцию, сгенерировать SQL-скрипт, применить          |
| 26  | `Pydantic v2 & Data Modeling` | Валидация, сериализация, custom validators, JSON Schema | `field_validator`, `model_config`, `computed_field`, `TypeAdapter` | Написать строгую модель с кастомной валидацией email/date      |
| 27  | `Кэширование стратегии`       | Ускорение ответов, инвалидация, distributed cache       | `redis.asyncio`, `cachetools`, HTTP caching, cache tags            | Кэшировать GET-эндпоинт, настроить invalidation по тегу        |

### 🏗️ Фаза 6: Архитектура, тестирование и качество (Модули 28-32)

| №   | Модуль                                 | Цель                                                     | Ключевые навыки                                            | Практика                                                 |
| --- | -------------------------------------- | -------------------------------------------------------- | ---------------------------------------------------------- | -------------------------------------------------------- |
| 28  | `Clean Architecture в Python`          | Слои, границы, dependency inversion, use cases           | `interfaces`, `use_cases`, `gateways`, DI container        | Рефакторить монолит в слои с чистой бизнес-логикой       |
| 29  | `CQRS & Mediator Patterns`             | Разделение чтение/запись, handlers, pipeline             | `Command/Query`, `Handler`, `PipelineBehavior`, validation | Реализовать команду `CreateOrderCommand` с валидацией    |
| 30  | `Тестирование с pytest`                | Fixtures, parametrization, mocking, hypothesis, coverage | `@pytest.fixture`, `mock`, `pytest-cov`, `hypothesis`      | Покрыть сервис тестами, параметризовать данные           |
| 31  | `Integration Testing & Testcontainers` | Реальная БД/Redis, `WebTestClient`, isolation            | `testcontainers`, `pytest-asyncio`, seed data, cleanup     | Написать интеграционный тест с PostgreSQL в Docker       |
| 32  | `Static Analysis & Type Safety`        | mypy, pyright, type guards, `typing.Protocol`            | `TypeGuard`, `Never`, `Self`, `@overload`, strict mode     | Включить `mypy --strict`, исправить все ошибки типизации |

### 🚀 Фаза 7: Production, деплой и продвинутые темы (Модули 33-38+)

| №   | Модуль                       | Цель                                                           | Ключевые навыки                                                 | Практика                                                 |
| --- | ---------------------------- | -------------------------------------------------------------- | --------------------------------------------------------------- | -------------------------------------------------------- |
| 33  | `Контейнеризация`            | Docker, multi-stage, uvicorn/gunicorn, non-root, health checks | `Dockerfile`, `.dockerignore`, `HEALTHCHECK`, `USER`, `WORKDIR` | Создать образ <150MB, запустить в Docker Compose         |
| 34  | `CI/CD & автоматизация`      | GitHub Actions, lint/test/build matrix, caching, OIDC          | YAML pipelines, `actions/setup-python`, `uv`, coverage reports  | Настроить pipeline: lint → test → build → deploy staging |
| 35  | `Observability & мониторинг` | OpenTelemetry, Prometheus, Grafana, tracing, metrics           | `opentelemetry-sdk`, `prometheus_client`, structlog, span       | Экспортировать метрики в Prometheus, создать дашборд     |
| 36  | `Performance tuning`         | Профилирование, pooling, async optimization, GC tuning         | `py-spy`, `memray`, connection pooling, `uvloop`                | Забенчмарить эндпоинт, оптимизировать аллокации/пулы     |
| 37  | `Security hardening`         | OWASP, secrets, CORS, rate limiting, dependency scanning       | `bandit`, `safety`, `pip-audit`, CSP/HSTS, input sanitization   | Просканировать зависимости, настроить security headers   |
| 38+ | `Продвинутые сценарии`       | WebAssembly/Pyodide, C-extensions, AI/ML, microservices, gRPC  | `pybind11`, `grpcio`, `langchain`, `microk8s`, service mesh     | Интегрировать LLM-ассистента, собрать gRPC-сервис        |

---

## 💻 Практические примеры кода (Python 3.11+)

### 🔹 1. Modern FastAPI + Pydantic v2 + DI + Async DB

```python
# app/main.py
from contextlib import asynccontextmanager
from typing import Annotated
from fastapi import FastAPI, Depends, HTTPException, status
from pydantic import BaseModel, Field, EmailStr, field_validator
from sqlalchemy.ext.asyncio import AsyncSession, create_async_engine, async_sessionmaker
from sqlalchemy import select
import structlog

logger = structlog.get_logger()

# 🔹 1. Pydantic v2 модель с валидацией
class UserCreate(BaseModel):
    email: EmailStr
    age: int = Field(ge=18, le=120)
    tags: list[str] = Field(default_factory=list)

    @field_validator("email", mode="before")
    @classmethod
    def normalize_email(cls, v: str) -> str:
        return v.lower().strip()

# 🔹 2. Async DB конфигурация
engine = create_async_engine("postgresql+asyncpg://user:pass@localhost/db", pool_size=10)
SessionLocal = async_sessionmaker(engine, expire_on_commit=False)

async def get_db() -> AsyncSession:
    async with SessionLocal() as session:
        yield session

# 🔹 3. Lifespan & Graceful Shutdown
@asynccontextmanager
async def lifespan(app: FastAPI):
    logger.info("Application starting up...")
    yield
    await engine.dispose()
    logger.info("Application shut down gracefully.")

app = FastAPI(lifespan=lifespan)

# 🔹 4. Endpoint с DI и валидацией
@app.post("/users", status_code=status.HTTP_201_CREATED)
async def create_user(
    user: UserCreate,
    db: Annotated[AsyncSession, Depends(get_db)]
):
    # Проверка существования
    result = await db.execute(select(User).where(User.email == user.email))
    if result.scalar_one_or_none():
        raise HTTPException(status_code=409, detail="Email already exists")

    db_user = User(**user.model_dump())
    db.add(db_user)
    await db.commit()
    await db.refresh(db_user)
    logger.info("User created", user_id=db_user.id, email=db_user.email)
    return {"id": db_user.id, "email": db_user.email}
```

### 🔹 2. Asyncio + httpx + Retry/Cancellation + Structured Logging

```python
# app/clients/external_api.py
import asyncio
from typing import Any
import httpx
from tenacity import retry, stop_after_attempt, wait_exponential, retry_if_exception_type
import structlog

logger = structlog.get_logger()

class ExternalAPIClient:
    def __init__(self, base_url: str, timeout: float = 10.0):
        self.base_url = base_url
        self.timeout = httpx.Timeout(timeout)

    @retry(
        stop=stop_after_attempt(3),
        wait=wait_exponential(multiplier=0.5, min=1, max=10),
        retry=retry_if_exception_type((httpx.ConnectError, httpx.ReadTimeout))
    )
    async def fetch_data(self, endpoint: str, params: dict[str, Any], ctx: asyncio.Context) -> dict:
        url = f"{self.base_url}/{endpoint}"
        log = logger.bind(endpoint=endpoint, url=url)

        async with httpx.AsyncClient(timeout=self.timeout) as client:
            log.info("Request started", params=params)
            try:
                response = await client.get(url, params=params)
                response.raise_for_status()
                data = response.json()
                log.info("Request succeeded", status=response.status_code, items=len(data.get("items", [])))
                return data
            except httpx.HTTPStatusError as e:
                log.error("HTTP error", status=e.response.status_code)
                raise
            except Exception as e:
                log.exception("Unexpected error")
                raise
```

### 🔹 3. SQLAlchemy 2.0 Async + Repository Pattern

```python
# app/repositories/user_repo.py
from sqlalchemy.ext.asyncio import AsyncSession
from sqlalchemy import select
from app.models import User

class UserRepository:
    def __init__(self, session: AsyncSession):
        self.session = session

    async def get_by_email(self, email: str) -> User | None:
        stmt = select(User).where(User.email == email)
        result = await self.session.execute(stmt)
        return result.scalar_one_or_none()

    async def create(self, user: User) -> User:
        self.session.add(user)
        await self.session.flush()
        await self.session.refresh(user)
        return user

    async def update_tags(self, user_id: int, new_tags: list[str]) -> User:
        user = await self.session.get(User, user_id)
        if not user:
            raise ValueError("User not found")
        user.tags = new_tags
        await self.session.flush()
        return user
```

### 🔹 4. Интеграционные тесты с pytest + Testcontainers

```python
# tests/test_users_api.py
import pytest
from httpx import AsyncClient, ASGITransport
from testcontainers.postgres import PostgresContainer
from app.main import app, get_db
from sqlalchemy.ext.asyncio import create_async_engine, async_sessionmaker

@pytest.fixture(scope="session")
def postgres():
    with PostgresContainer("postgres:16-alpine") as pg:
        yield pg.get_connection_url()

@pytest.fixture
async def db_session(postgres: str):
    engine = create_async_engine(postgres)
    async with engine.begin() as conn:
        await conn.run_sync(Base.metadata.create_all)  # Имитация миграций

    async_session = async_sessionmaker(engine, expire_on_commit=False)
    async with async_session() as session:
        yield session

    async with engine.begin() as conn:
        await conn.run_sync(Base.metadata.drop_all)
    await engine.dispose()

@pytest.mark.asyncio
async def test_create_user_success(db_session, monkeypatch):
    # Подменяем зависимость БД на тестовую сессию
    async def override_get_db():
        yield db_session

    app.dependency_overrides[get_db] = override_get_db

    transport = ASGITransport(app=app)
    async with AsyncClient(transport=transport, base_url="http://test") as client:
        response = await client.post("/users", json={
            "email": "test@example.com",
            "age": 25,
            "tags": ["early_adopter"]
        })

        assert response.status_code == 201
        data = response.json()
        assert data["email"] == "test@example.com"
        assert "id" in data
```

### 🔹 5. Middleware + Rate Limiting + OpenTelemetry Tracing

```python
# app/middleware.py
from fastapi import Request, Response
from fastapi.responses import JSONResponse
from opentelemetry import trace
from opentelemetry.instrumentation.fastapi import FastAPIInstrumentor
import time

tracer = trace.get_tracer("api.tracer")

async def log_request_middleware(request: Request, call_next):
    start = time.perf_counter()
    span = tracer.start_as_current_span(
        f"{request.method} {request.url.path}",
        attributes={
            "http.method": request.method,
            "http.url": str(request.url),
            "http.client_ip": request.client.host if request.client else "unknown"
        }
    )
    try:
        response = await call_next(request)
        span.set_attribute("http.status_code", response.status_code)
        return response
    except Exception as e:
        span.record_exception(e)
        span.set_status(trace.StatusCode.ERROR)
        raise
    finally:
        duration_ms = (time.perf_counter() - start) * 1000
        span.set_attribute("http.duration_ms", duration_ms)
        span.end()

# Инициализация трассировки
FastAPIInstrumentor.instrument_app(app)
app.middleware("http")(log_request_middleware)
```

---

## ✅ Production-чек-лист

| Аспект            | Вопрос                                                                      | Да/Нет |
| ----------------- | --------------------------------------------------------------------------- | ------ |
| `Типизация`       | Включён ли `mypy --strict`/`pyright`, покрыты ли модели Pydantic v2?        | ☐      |
| `Async I/O`       | Нет ли `requests`/`time.sleep` в async-коде, все БД/HTTP вызовы асинхронны? | ☐      |
| `DI & Lifespan`   | Используется ли `lifespan` для startup/shutdown, DI через `Depends`?        | ☐      |
| `Безопасность`    | Настроены ли CORS, rate limiting, JWT validation, secrets из env/vault?     | ☐      |
| `Логирование`     | Используются ли structured logs (structlog), корреляция `trace_id`, уровни? | ☐      |
| `Тесты`           | Покрыты ли endpoints интеграционными тестами с `testcontainers`?            | ☐      |
| `Миграции`        | Генерируются ли alembic-скрипты, тестируется ли rollback/expansion?         | ☐      |
| `Контейнеризация` | Docker non-root, multi-stage, health checks, `uvicorn --workers`?           | ☐      |
| `Observability`   | Включены ли OpenTelemetry, Prometheus metrics, tracing, `/health`?          | ☐      |
| `CI/CD`           | Автоматизированы ли lint/test/build/deploy, caching, coverage reports?      | ☐      |

---

## ❌ Типичные ошибки и антипаттерны

| Ошибка                             | Последствие                                            | Как исправить                                                                   |
| ---------------------------------- | ------------------------------------------------------ | ------------------------------------------------------------------------------- |
| ``requests` в async-приложении``   | Блокировка event loop, thread pool starvation          | Использовать `httpx.AsyncClient` или `anyio.to_thread`                          |
| `Глобальные мутабельные состояния` | Гонки данных, неконсистентность в multi-worker         | Выносить в Redis/БД, использовать `app.state` только для read-only              |
| ``except:` без контекста``         | Теряются стектрейсы, невозможно дебажить               | Всегда `except Exception as e:`, логировать `logger.exception()`                |
| `Pydantic v1 стиль в v2`           | Падение производительности, конфликт с `model_config`  | Мигрировать на `@field_validator`, `model_validate`, `TypeAdapter`              |
| `SQLAlchemy 1.x стиль`             | N+1, `session.query()`, `commit()` без flush           | Перейти на 2.0 style: `select()`, `session.execute()`, `expire_on_commit=False` |
| `Singleton DB connection`          | Гонки, утечки соединений, cross-request state          | Регистрировать `AsyncSession` как scoped/dependency, использовать pool          |
| ``Синхронные `time.sleep()```      | Блокировка event loop, таймауты                        | Заменить на `await asyncio.sleep()`                                             |
| ``Игнорирование `mypy`/`pyright``` | Runtime TypeError, рефакторинг = страх                 | Включить `strict = true`, использовать `TypeGuard`, `Protocol`, `Never`         |
| `In-memory cache в multi-worker`   | Рассинхрон, потеря данных при restart                  | Использовать `redis.asyncio`, `dogpile.cache`, или external cache               |
| `Миграции в runtime`               | Блокировки при старте, падение при параллельном деплое | Генерировать SQL `alembic upgrade --sql`, применять в CI/CD отдельно            |

---

## 📚 Ресурсы и ссылки

### 🔹 Официальные & авторитетные

| Ресурс                                                    | Описание                                              |
| --------------------------------------------------------- | ----------------------------------------------------- |
| [Python Documentation](https://docs.python.org/3/)        | Язык, stdlib, asyncio, typing, best practices         |
| [FastAPI Documentation](https://fastapi.tiangolo.com/)    | Routing, DI, security, testing, production deployment |
| [SQLAlchemy 2.0 Docs](https://docs.sqlalchemy.org/en/20/) | Core, ORM, async, 2.0 style guide, performance        |
| [Pydantic v2 Docs](https://docs.pydantic.dev/latest/)     | Validation, serialization, JSON Schema, settings      |
| [pytest Docs](https://docs.pytest.org/en/stable/)         | Fixtures, parametrization, mocking, plugins           |

### 🔹 Книги & курсы

| Ресурс                                              | Фокус                                                          |
| --------------------------------------------------- | -------------------------------------------------------------- |
| `Fluent Python (Luciano Ramalho)`                   | Глубокое понимание языка, data model, descriptors, metaclasses |
| `Architecture Patterns with Python (Cosmic Python)` | DDD, CQRS, event sourcing, testing, boundaries                 |
| `High-Performance Python`                           | Profiling, concurrency, memory, Cython, Numba                  |
| `Test-Driven Development with Python`               | pytest, Django/Flask/FastAPI testing, BDD                      |
| `Microsoft Learn / Real Python`                     | Интерактивные модули, production guides, security              |

### 🔹 Инструменты

| Инструмент              | Назначение                                                 |
| ----------------------- | ---------------------------------------------------------- |
| `uv`                    | Быстрый менеджер пакетов/резольвер (замена pip/poetry)     |
| `ruff`                  | Мгновенный линтер/форматтер (замена flake8/isort/black)    |
| `mypy / pyright`        | Статическая типизация, strict mode, type narrowing         |
| `testcontainers-python` | Integration testing с реальными БД/Redis/RabbitMQ в Docker |
| `opentelemetry-python`  | Tracing, metrics, logging export to Prometheus/Jaeger      |

---

## 🎯 Заключение + план действий

> ✅ **Главное правило**: `Python production = типизация + async + тесты + observability`. Не пишите скрипты для веба. Используйте Pydantic v2 для валидации, SQLAlchemy 2.0 для БД, FastAPI для маршрутизации, pytest + testcontainers для тестов, OpenTelemetry для мониторинга. Когда asyncio не справляется — используйте `uvloop`/`anyio`. Когда ORM тормозит — `asyncpg`/`Dapper-style` raw queries. Когда синхронный код блокирует — вынесите в thread pool или переделайте на async.

### 📅 Рекомендуемый темп

| Недели | Фокус    | Результат                                                  |
| ------ | -------- | ---------------------------------------------------------- |
| 1-2    | Фаза 1-2 | Современный тулинг, типизация, рантайм, memory/GC          |
| 3-4    | Фаза 3-4 | Asyncio, httpx, FastAPI, auth, middleware, DI              |
| 5-6    | Фаза 5-6 | SQLAlchemy 2.0, Pydantic v2, тесты, clean architecture     |
| 7-8    | Фаза 7   | Docker, CI/CD, OpenTelemetry, performance, security        |
| 9+     | Фаза 8+  | CQRS, gRPC, AI integration, microservices, advanced tuning |

### 🔹 Что делать прямо сейчас

1. `Установите uv`: `curl -LsSf https://astral.sh/uv/install.sh | sh`
2. `Создайте проект`: `uv init myapi && cd myapi && uv add "fastapi[standard]" httpx pydantic structlog`
3. `Включите линтер/тайпер`: `uv add --dev ruff mypy pytest pytest-asyncio`
4. `Напишите первый endpoint` с Pydantic v2, DI, structured logs
5. `Запустите`: `uv run fastapi dev main.py`, откройте `/docs`, протестируйте
6. `Добавьте тест`: `uv run pytest -v`, настройте `pytest.ini`

📌 **Запомните**: `Python` — это `платформа для production`, а не только "скриптовый язык". Начинайте с малого: один endpoint, одна модель, один тест. Добавляйте типизацию, async, DI, логирование, деплой. Каждый шаг — `инвестиция в предсказуемость`: сегодня вы тратите час на настройку `mypy --strict`, завтра получаете код, который не падает в runtime, держит 10K RPS, логируется в JSON и деплоится в контейнере за 20 секунд. Документируйте: «типы = strict, I/O = async, валидация = Pydantic v2, БД = SQLAlchemy 2.0, тесты = testcontainers, мониторинг = OpenTelemetry» — это ваша `архитектурная конституция` для production-ready Python-разработки.
