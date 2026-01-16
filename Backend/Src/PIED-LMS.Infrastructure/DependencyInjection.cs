using PIED_LMS.Infrastructure.Options;
using PIED_LMS.Infrastructure.Persistence;

namespace PIED_LMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));

        var dbOptions = configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>();
        if (dbOptions is null || string.IsNullOrEmpty(dbOptions.Host))
        {
            throw new InvalidOperationException("Database configuration is missing or invalid.");
        }
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(dbOptions?.ConnectionString)
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
