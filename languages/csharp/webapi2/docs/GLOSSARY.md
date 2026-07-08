# Glossary — Термины курса WebAPI2

## Общие

| Термин | Определение |
|--------|-------------|
| **Boilerplate** | Стартовый шаблон микросервиса. Содержит инфраструктуру (middleware, DI, logging, health checks) без бизнес-логики. Можно клонировать и наполнять под конкретный проект. |
| **Trade-off** | Компромисс — выбор между двумя вариантами, где каждый имеет плюсы и минусы. Нет идеального решения, только подходящее под контекст. |
| **Minimal but sufficient** | Принцип: делать минимально достаточную реализацию. Не overengineering, но и не "голый" проект. |
| **DDD (Domain-Driven Design)** | Подход проектирования, где структура кода отражает бизнес-домен. Ключевые элементы: Aggregates, Entities, Value Objects, Domain Services. |
| **Middleware** | Компонент обрабатывающий HTTP-запросы в pipeline. Порядок middleware критичен. |
| **DI Container (Dependency Injection)** | Контейнер управляет зависимостями между классами. Lifetime: Transient, Scoped, Singleton. |
| **Fail-fast** | Принцип: падать при ошибках конфигурации при старте, а не в runtime. |
| **Configuration Validation** | Проверка конфигурации при старте приложения. Используется `ValidateOnStart()`. |
| **Scalar** | Современный OpenAPI UI. Альтернатива Swagger. Быстрее, компактнее, активнее поддерживается. |
| **Result<T>** | Собственный паттерн результата. Единый контракт: успех (Value) или ошибка (Error). Без лишних зависимостей. |
| **Shared.Config** | Отдельный проект для общей конфигурации. Все конфигураторы вынесены сюда. Program.cs — только вызовы. |
| **Configurator** | Класс в Shared.Config, отвечающий за одну область настройки (logging, auth, middleware и т.д.). |
| **ADR (Architecture Decision Record)** | Документ фиксирующий архитектурное решение: что выбрали, почему, какие альтернативы рассматривали. |

## DDD (Domain-Driven Design)

| Термин | Определение |
|--------|-------------|
| **Aggregate** | Группа объектов, обрабатываемых как единое целое для изменения данных. Содержит Aggregate Root — главную Entity. |
| **Entity** | Объект с уникальным ID. Состояние меняется, но ID不变. Пример: User с Id=1. |
| **Value Object** | Объект без ID, определяется свойствами. Неизменяемый (immutable). Пример: Email("user@mail.com"). |
| **Domain Service** | Логика, не принадлежащая ни одной Entity. Пример: TransferService для перевода между счетами. |
| **Domain Event** | Событие, произошедшее в домене. Пример: OrderCreated, PaymentProcessed. |
| **Aggregate Root** | Главная Entity в Aggregate. Все изменения проходят через неё. Внешние объекты ссылаются только на Root. |
| **Domain Layer** | Самый внутренний слой DDD. Содержит ядро бизнес-логики. Не зависит ни от какого другого слоя. |
| **Application Layer** | Слой Use Cases. Содержит интерфейсы, оркестрацию. Зависит от Domain. |
| **Infrastructure Layer** | Самый внешний слой. Реализует интерфейсы из Application/Domain (EF Core, внешние сервисы). |
| **BaseEntity** | Базовый класс для Entity. Содержит Id, CreatedAt, UpdatedAt. |
| **BaseAggregateRoot** | Базовый класс для Aggregate Root. Наследует BaseEntity + Domain Events. |
| **BaseValueObject** | Базовый класс для Value Object. Реализует Equals, GetHashCode, operator==. |
