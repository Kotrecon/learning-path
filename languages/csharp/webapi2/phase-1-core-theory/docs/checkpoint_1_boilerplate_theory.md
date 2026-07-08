# Checkpoint 1: Boilerplate Theory

## Объясни за 2 минуты

**Boilerplate** — это стартовый шаблон микросервиса. Содержит инфраструктуру (middleware, DI, logging, health checks) без бизнес-логики. Можно клонировать и наполнять под конкретный проект.

**Зачем нужен**:
- Не писать одно и то же каждый раз
- Единый стандарт для команды
- Знать каждую строку кода
- Быстрый старт нового микросервиса

**Trade-offs**:
- Custom: контроль, но время
- Готовый: скорость, но избыточность
- Гибрид: баланс (recommended)

**Структура DDD**:
- WebApi → Application → Domain
- Domain: самый внутренний, не зависит ни от кого
- Shared.Config: конфигурация + конфигураторы

**Middleware pipeline**:
- Порядок критичен!
- Exception Handler → HSTS → HTTPS → CORS → Rate Limiting → Auth → Routing

**Fail-fast**:
- Проверка наличия секций
- DataAnnotations
- ValidateOnStart()
- ConfigurationValidator

---

## Checkpoint: Pass/Fail

| Критерий | Pass | Fail |
|----------|------|------|
| Объяснить boilerplate за 2 минуты | Да, с примерами | Только теория |
| Понимать trade-offs custom vs готовый шаблон | Да, может объяснить | Не знает разницы |
| Спроектировать структуру решения для микросервиса с DDD | Да, с диаграммой зависимостей | Нет структуры |
| Понимать DDD концепции | Да, может объяснить | Путает с Clean Architecture |
| Понимать middleware pipeline и порядок | Да, может объяснить порядок | Не знает порядок |
| Понимать fail-fast принцип | Да, может объяснить | Не знает fail-fast |
| Понимать configuration validation | Да, может объяснить | Не знает configuration validation |
| Написать ADR-000 и ADR-001 | Оба ADR | Меньше 1 |
| Знать 7 Common mistakes | Все 7 | Меньше 4 |

---

## Артефакты Phase 1

| Файл | Описание |
|------|----------|
| `ADR-000-why-custom-boilerplate.md` | Почему выбираем custom boilerplate с DDD |
| `ADR-001-ddd-structure.md` | DDD структура для микросервиса |
| `solution_structure.md` | Структура решения + Mermaid диаграмма |
| `middleware_pipeline.md` | Middleware pipeline + Mermaid диаграмма |
| `fail_fast.md` | Fail-fast principle + 4 уровня валидации |
| `checkpoint_1_boilerplate_theory.md` | Этот документ |

---

## 7 ключевых ошибок boilerplate

1. **Secrets в коде** — пароли, ключи в Git
2. **Неправильный порядок middleware** — security issues
3. **Captive dependency** — scoped в singleton
4. **Overengineering** — слишком сложный boilerplate
5. **Нет health checks** — невозможно мониторить
6. **Debug middleware в production** — утечка информации
7. **Нет configuration validation** — падение в runtime
