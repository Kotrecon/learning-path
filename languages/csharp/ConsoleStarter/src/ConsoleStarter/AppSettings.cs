namespace ConsoleStarter;

public record AppSettings
{
    public string Name { get; init; } = string.Empty;
    public int Timeout { get; init; }
}