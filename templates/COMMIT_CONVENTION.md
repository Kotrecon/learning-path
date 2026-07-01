# Конвенции коммитов

## Формат

```bash
<type>(<scope>): <описание>
```

## Типы

| Тип        | Когда использовать                   | Пример                                                |
| ---------- | ------------------------------------ | ----------------------------------------------------- |
| `feat`     | Новая функциональность               | `feat(module9.1): enable TraceId injection in logs`   |
| `fix`      | Исправление бага                     | `fix(module8.3): resolve counter not incrementing`    |
| `chore`    | Обслуживание, настройка, рефакторинг | `chore(module8.5): configure OTLP gRPC endpoint`      |
| `docs`     | Документация                         | `docs(module8.6): add module README`                  |
| `refactor` | Рефакторинг без изменения логики     | `refactor(module9.2): extract correlation to service` |

## Scope

| Scope         | Формат          | Пример                   |
| ------------- | --------------- | ------------------------ |
| Задача модуля | `module<N>.<M>` | `module9.1`, `module8.3` |
| Весь модуль   | `module<N>`     | `module9`                |
| Фаза          | `phase<N>`      | `phase2`                 |

## Описание

- Глагол в imperativo: `enable`, `add`, `configure`, `implement`, `resolve`
- Без точки в конце
- На английском
- Максимум 72 символа

## Примеры

```bash
feat(module9.1): enable TraceId injection in logs
feat(module9.2): implement CorrelationId logging
chore(module9): add skeleton files (theory, task, progress, readme)
docs(module9.6): add module README and finalize progress tracker
docs(phase2): mark module9 as completed in phase tracker
fix(module9.3): resolve Baggage not propagating across async
refactor(module9.4): extract correlation logic to helper
```

## Порядок коммитов в модуле

1. `chore(module<N>): add skeleton files` — создание структуры
2. `feat(module<N>.1): ...` — задача 1
3. `feat(module<N>.2): ...` — задача 2
4. ...
5. `docs(module<N>.6): ...` — документация и финализация
6. `docs(phase<N>): mark module<N> as completed` — обновление трекера фазы
