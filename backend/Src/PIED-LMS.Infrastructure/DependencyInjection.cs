using PIED_LMS.Application.Services;
using PIED_LMS.Infrastructure.Identity;
using PIED_LMS.Infrastructure.Options;
using PIED_LMS.Infrastructure.Persistence;
using PIED_LMS.Infrastructure.Services;

namespace PIED_LMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // CORS for development
        services.AddCors(options =>
        {
            options.AddPolicy("Development", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });

            options.AddPolicy("Production", policy =>
            {
                policy.WithOrigins("https://pied-lms.com")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });

        services.AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection(DatabaseOptions.SectionName))
            .Validate(opt => !string.IsNullOrWhiteSpace(opt.Host), "Database: Host is required")
            .Validate(opt => opt.Port is > 0 and <= 65535, "Database: Port must be between 1 and 65535")
            .Validate(opt => !string.IsNullOrWhiteSpace(opt.Name), "Database: Name is required")
            .Validate(opt => !string.IsNullOrWhiteSpace(opt.User), "Database: User is required")
            .Validate(opt => !string.IsNullOrWhiteSpace(opt.Password), "Database: Password is required")
            .ValidateOnStart();

        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(opt => !string.IsNullOrWhiteSpace(opt.Issuer), "JWT: Issuer is required")
            .Validate(opt => !string.IsNullOrWhiteSpace(opt.Audience), "JWT: Audience is required")
            .Validate(opt => !string.IsNullOrWhiteSpace(opt.SecretKey), "JWT: SecretKey is required")
            .Validate(opt => opt.SecretKey.Length >= 32, "JWT: SecretKey must be at least 32 characters")
            .Validate(opt => opt.ExpiryMinutes > 0, "JWT: ExpiryMinutes must be greater than 0")
            .Validate(opt => opt.RefreshTokenExpiryDays > 0, "JWT: RefreshTokenExpiryDays must be greater than 0")
            .ValidateOnStart();

        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var dbOptions = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            options.UseNpgsql(dbOptions.ConnectionString)
                .UseSnakeCaseNamingConvention();
        });

        services.AddIdentity<UserProfile, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

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

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()!;

                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
