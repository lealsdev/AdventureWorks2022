using Scalar.AspNetCore;
using People.Infrastructure.Configuration;
using People.Application.Configuration;
using People.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioningConfiguration(builder.Configuration);
builder.Services.AddApplicationConfiguration();
builder.Services.AddInfrastructureConfiguration(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
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
