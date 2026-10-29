using Scalar.AspNetCore;
using People.Infrastructure.Configuration;
using People.Application.Configuration;
using People.Api.Extensions;
using People.Api.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioningConfiguration(builder.Configuration);
builder.Services.AddApplicationConfiguration();
builder.Services.AddInfrastructureConfiguration(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.PersistentAuthentication = true;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
