using PIED_LMS.API;
using PIED_LMS.API.Filters;
using PIED_LMS.Application;
using PIED_LMS.Infrastructure;
using PIED_LMS.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options => { options.Filters.Add<ValidationFilter>(); });
builder.Services.AddOpenApi();
builder.Services.AddAuthorization();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Only run migrations in development or if explicitly configured
var runMigrationsOnStartup = app.Configuration.GetValue<bool>("RunMigrationsOnStartup", app.Environment.IsDevelopment());
if (runMigrationsOnStartup)
{
    app.Logger.LogInformation("Running database migrations on startup (Environment: {Environment})", app.Environment.EnvironmentName);
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
    }
    app.Logger.LogInformation("Database migrations completed successfully");
}
else
{
    app.Logger.LogInformation("Skipping automatic database migrations (set RunMigrationsOnStartup to enable)");
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/docs", options => { options.WithTitle("PIED-LMS API").WithTheme(ScalarTheme.Kepler); });
    app.UseCors("Development");
}
else
{
    app.UseCors("Production");
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapCustomEndpoints();

await app.RunAsync();
