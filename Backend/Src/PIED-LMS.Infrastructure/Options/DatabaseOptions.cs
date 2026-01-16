using System.ComponentModel.DataAnnotations;

namespace PIED_LMS.Infrastructure.Options;

public class DatabaseOptions
{
    public const string SectionName = "Database";

    [Required]
    public string Host { get; init; } = string.Empty;

    [Range(1, 65535)]
    public int Port { get; init; } = 5432;

    [Required]
    public string Name { get; init; } = string.Empty;

    [Required]
    public string User { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;

    public string ConnectionString =>
        $"Host={Host};Port={Port};Database={Name};Username={User};Password={Password};";
}
