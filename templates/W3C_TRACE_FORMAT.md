# W3C TraceContext Format

## traceparent

Формат заголовка для передачи контекста трассировки:

```csharp
00-{trace-id}-{parent-id}-{trace-flags}
```

### Поля

| Поле        | Описание                                       | Формат          | Пример                             |
| ----------- | ---------------------------------------------- | --------------- | ---------------------------------- |
| version     | Версия спецификации                            | 2 hex-символа   | `00`                               |
| trace-id    | Уникальный идентификатор трассировки (128 бит) | 32 hex-символа  | `3ddacca42b50beb0b003b19c3239f53b` |
| parent-id   | Идентификатор текущего спана (64 бита)         | 16 hex-символов | `b59d3738ddd5770e`                 |
| trace-flags | Флаги трассировки (01 = sampled)               | 2 hex-символа   | `01`                               |

### Пример

```csharp
00-3ddacca42b50beb0b003b19c3239f53b-b59d3738ddd5770e-01
```

### Валидация

- `trace-id` не должен быть нулевым (`00000000000000000000000000000000`)
- `parent-id` не должен быть нулевым (`0000000000000000`)
- Длина строки ровно 55 символов

---

## tracestate

Дополнительный заголовок для передачи.vendor-специфичного контекста:

```csharp
vendorname1=tracestate1,vendorname2=tracestate2
```

### Пример

```csharp
rojo=00f067aa0ba902b7;congo=t61rcWkgMzE
```

---

## baggage

Заголовок для передачи кастомных пар key-value:

```csharp
key1=value1,key2=value2
```

### Ограничения

- Максимум 180 пар (после обрезки)
- Ключ: 1-256 символов, без `=`, `,`, пробелов, контрольных символов
- Значение: 0-4096 символов, URL-encoded

### Пример

```csharp
user.id=123,tenant.id=abc
```

---

## Ссылки

- [W3C TraceContext Spec](https://www.w3.org/TR/trace-context/)
- [TraceId](https://www.w3.org/TR/trace-context/#trace-id)
- [Baggage](https://www.w3.org/TR/baggage/)

---
