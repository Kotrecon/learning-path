# Fail-Fast Principle

## Суть

Приложение **не должно стартовать**, если конфигурация невалидна. Лучше упасть сразу, чем в runtime при первом запросе.

## Fail-fast vs Non-fail-fast

```mermaid
graph TD
    subgraph "FAIL-FAST"
        A1[Start App] --> B1[Validate Config]
        B1 --> C1{Valid?}
        C1 -->|Yes| D1[Run App]
        C1 -->|No| E1[Exception at Startup]
    end

    subgraph "NON-FAIL-FAST"
        A2[Start App] --> B2[Run App]
        B2 --> C2[Request]
        C2 --> D2[Use Config]
        D2 --> E2{Valid?}
        E2 -->|Yes| F2[Response]
        E2 -->|No| G2[Exception in Runtime]
    end

    style E1 fill:#ff6b6b,color:#fff
    style G2 fill:#ff6b6b,color:#fff
    style D1 fill:#4ecdc4,color:#fff
    style F2 fill:#4ecdc4,color:#fff
```

## Примеры

| Сценарий | Fail-fast | Non-fail-fast |
|----------|-----------|---------------|
| Нет connection string | Приложение не стартует | Падает при первом запросе к БД |
| Невалидный JWT key | Приложение не стартует | Падает при первой аутентификации |
| Недоступен Redis | Приложение не стартует | Падает при первом обращении к кешу |
| Нет секции конфигурации | Приложение не стартует | Падает при использовании |

## 4 уровня валидации конфигурации

```mermaid
graph TD
    A[appsettings.json] --> B{УРОВЕНЬ 1: Проверка секций}
    B -->|Секция есть| C{УРОВЕНЬ 2: DataAnnotations}
    B -->|Секции нет| D[Exception: Секция отсутствует]
    C -->|Валидно| E{УРОВЕНЬ 3: ValidateOnStart}
    C -->|Невалидно| F[Exception: Невалидные значения]
    E -->|OK| G{УРОВЕНЬ 4: ConfigurationValidator}
    E -->|Ошибка| H[Exception при старте]
    G -->|Валидно| I[App Running]
    G -->|Невалидно| J[Exception: Кастомная валидация]

    style D fill:#ff6b6b,color:#fff
    style F fill:#ff6b6b,color:#fff
    style H fill:#ff6b6b,color:#fff
    style J fill:#ff6b6b,color:#fff
    style I fill:#4ecdc4,color:#fff
```

### Уровень 1: Проверка наличия секций

```csharp
// Проверяем что каждая секция существует
var jwtSection = configuration.GetSection("Jwt");
if (!jwtSection.Exists())
    throw new InvalidOperationException("Секция 'Jwt' отсутствует в конфигурации");

var dbSection = configuration.GetSection("Database");
if (!dbSection.Exists())
    throw new InvalidOperationException("Секция 'Database' отсутствует в конфигурации");
```

### Уровень 2: DataAnnotations

```csharp
public sealed class JwtSettings
{
    [Required(ErrorMessage = "JWT Key обязателен")]
    public string Key { get; set; } = null!;

    [Required(ErrorMessage = "JWT Issuer обязателен")]
    public string Issuer { get; set; } = null!;

    [Range(1, 1440, ErrorMessage = "ExpireMinutes: от 1 до 1440")]
    public int ExpireMinutes { get; set; } = 60;
}
```

### Уровень 3: ValidateOnStart

```csharp
builder.Services.AddOptions<JwtSettings>()
    .BindConfiguration("Jwt")
    .ValidateDataAnnotations()
    .ValidateOnStart();  // Валидация при старте, не в runtime!
```

### Уровень 4: ConfigurationValidator

```csharp
public sealed class ConfigurationValidator
{
    private readonly IConfiguration _configuration;

    public ConfigurationValidationResult Validate()
    {
        var errors = new List<string>();

        // Проверка connection string
        var connString = _configuration.GetConnectionString("Default");
        if (string.IsNullOrEmpty(connString))
            errors.Add("Connection string 'Default' отсутствует");

        // Проверка JWT key
        var jwtKey = _configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey) || jwtKey.Length < 32)
            errors.Add("JWT Key должен быть не менее 32 символов");

        return new ConfigurationValidationResult(errors);
    }
}
```

## Где в Program.cs происходит валидация

```mermaid
graph TD
    A[Program.cs] --> B[ConfigureLogging]
    B --> C[ConfigureConfiguration]
    C --> D[ConfigureServices]
    D --> E[ConfigureAuthentication]
    E --> F[ConfigureAuthorization]
    F --> G[ConfigureHealthChecks]
    G --> H[ConfigureApiDocumentation]
    H --> I[ConfigureCors]
    I --> J[ConfigureRateLimiting]
    J --> K[Build]
    K --> L[UseMiddlewarePipeline]
    L --> M[Run]

    style C fill:#ff6b6b,color:#fff
    style D fill:#ff6b6b,color:#fff
```

Конфигурация валидируется на шаге **ConfigureConfiguration** (самый ранний stage).
