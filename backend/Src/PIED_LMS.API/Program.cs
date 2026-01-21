using Microsoft.AspNetCore.RateLimiting;
using PIED_LMS.API.Middlewares;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.WriteTo.Console());
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddCarter();
builder.Services.AddHealthChecks();
builder.Services.AddResponseCaching();
builder.Services.AddRateLimiter(options => options.AddFixedWindowLimiter("health-policy", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromSeconds(10);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 2;
    }));
var app = builder.Build();
app.UseExceptionHandler();

app.UseSerilogRequestLogging();
app.UseRateLimiter();
app.UseResponseCaching();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCarter();
app.UseHttpsRedirection();

await app.RunAsync();
