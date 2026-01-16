var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/docs", options =>
    {
        options.WithTitle("PIED-LMS API").WithTheme(ScalarTheme.Kepler);
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));
if (app.Environment.IsDevelopment())
{
    app.MapGet("/", () => Results.Redirect("/docs"));
}
else
{
    app.MapGet("/", () => Results.Ok(new { message = "PIED-LMS API is running." }));
}
await app.RunAsync();
