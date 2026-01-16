using PIED_LMS.Infrastructure.Options;
using PIED_LMS.Infrastructure.Persistence;

namespace PIED_LMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection(DatabaseOptions.SectionName))
            .Validate(opt => !string.IsNullOrWhiteSpace(opt.Host), "Database: Host is required")
            .Validate(opt => opt.Port > 0 && opt.Port <= 65535, "Database: Port must be between 1 and 65535")
            .Validate(opt => !string.IsNullOrWhiteSpace(opt.Name), "Database: Name is required")
            .Validate(opt => !string.IsNullOrWhiteSpace(opt.User), "Database: User is required")
            .Validate(opt => !string.IsNullOrWhiteSpace(opt.Password), "Database: Password is required")
            .ValidateOnStart();

        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var dbOptions = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<DatabaseOptions>>().Value;
            options.UseNpgsql(dbOptions.ConnectionString)
                .UseSnakeCaseNamingConvention();
        });

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.User.RequireUniqueEmail = true;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
        });

        return services;
    }
}
