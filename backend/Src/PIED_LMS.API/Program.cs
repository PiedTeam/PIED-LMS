using PIED_LMS.API;
using PIED_LMS.Application;
using PIED_LMS.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.WriteTo.Console());

builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseInfrastructure();

await app.RunAsync();
