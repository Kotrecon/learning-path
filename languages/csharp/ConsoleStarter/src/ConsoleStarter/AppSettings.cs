using System.ComponentModel.DataAnnotations;

namespace ConsoleStarter;

public record AppSettings
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; init; } = string.Empty;

    [Range(1, 3600, ErrorMessage = "Timeout must be between 1 and 3600 seconds")]
    public int Timeout { get; init; }
}