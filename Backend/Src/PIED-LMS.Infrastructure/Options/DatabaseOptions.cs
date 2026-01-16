namespace PIED_LMS.Infrastructure.Options;

public class DatabaseOptions
{
    public const string SectionName = "Database";

    public string Host { get; init; } = string.Empty;
    public int Port { get; init; } = 5432;
    public string Name { get; init; } = string.Empty;
    public string User { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;

    public string ConnectionString =>
        $"Host={Host};Port={Port};Database={Name};Username={User};Password={Password};";
}
