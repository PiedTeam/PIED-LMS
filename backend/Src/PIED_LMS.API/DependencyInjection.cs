using PIED_LMS.API.Middlewares;

namespace PIED_LMS.API;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token (without 'Bearer' prefix)"
            });

            options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("Bearer", null, null),
                    new List<string>()
                }
            });
        });

        // 2. Exception Handling & Common Services
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddCarter();
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy("API is reachable"));
        services.AddResponseCaching();

        // 3. Rate Limiting
        services.AddRateLimiter(options => options.AddFixedWindowLimiter("health-policy", limiterOptions =>
        {
            limiterOptions.PermitLimit = 5;
            limiterOptions.Window = TimeSpan.FromSeconds(10);
            limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            limiterOptions.QueueLimit = 2;
        }));

        // 4. Authentication & Authorization
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey missing.");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization();

        return services;
    }

    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseSerilogRequestLogging();
        app.UseRateLimiter();
        app.UseResponseCaching();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapCarter();

        return app;
    }
}
