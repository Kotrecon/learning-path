# ADR-001: DDD структура для микросервиса

## Статус

Принято

## Контекст

Нужно спроектировать структуру решения для микросервиса. Варианты:

1. **Чистый WebApi** — один проект, всё в одном
2. **Clean Architecture** — слои с правилами зависимостей
3. **DDD (Domain-Driven Design)** — слои, отражающие бизнес-домен

## Решение

Выбираем **DDD структуру с 5 проектами**.

## Структура

```
src/
├── WebApi/           ← Presentation (контроллеры, middleware)
├── Application/      ← Use Cases (интерфейсы)
├── Domain/           ← Ядро (Aggregates, Entities, Value Objects)
├── Infrastructure/   ← Реализация (EF Core, внешние сервисы)
└── Shared.Config/    ← Конфигурация + конфигураторы
```

## Правила зависимостей

```
WebApi → Application → Domain
WebApi → Infrastructure → Application, Domain
WebApi → Shared.Config

Domain → (ни от кого)       ← ядро, независимо
Shared.Config → (ни от кого) ← конфигурация, независима
```

## Почему DDD

- **Структура отражает домен** — код понятен бизнесу
- **Domain Layer независим** — ядро защищено от изменений
- **Чёткие границы** — каждый слой имеет свою ответственность
- **Тестируемость** — можно тестировать каждый слой отдельно

## Почему 5 проектов, а не 1

- **Изоляция** — Domain не знает о EF Core
- **Гибкость** — можно заменить Infrastructure без изменения Domain
- **Тестируемость** — тесты для каждого слоя отдельно

## Альтернативы (что отвергли)

| Альтернатива | Почему отвергли |
|--------------|-----------------|
| Чистый WebApi | Нет изоляции, всё смешано |
| Clean Architecture | Избыточно для boilerplate, DDD проще |

## Ключевые классы

| Класс | Назначение |
|-------|-----------|
| **BaseEntity** | Базовый для Entity (Id, CreatedAt, UpdatedAt) |
| **BaseAggregateRoot** | Базовый для Aggregate Root (+ Domain Events) |
| **BaseValueObject** | Базовый для Value Object (Equals, GetHashCode) |
| **DomainException** | Базовое исключение домена |
