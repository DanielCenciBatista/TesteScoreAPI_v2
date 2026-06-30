using Teste.ScoreAPI.Api.Middleware;
using Teste.ScoreAPI.Application.Interfaces;
using Teste.ScoreAPI.Application.Services;
using Teste.ScoreAPI.Domain.Interfaces;
using Teste.ScoreAPI.Infrastructure.Data;
using Teste.ScoreAPI.Infrastructure.Repositories;
using Teste.ScoreAPI.Infrastructure.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<SqlConnectionFactory>();
builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddScoped<ICustomerRepository, SqlCustomerRepository>();
builder.Services.AddScoped<ICpfValidator, CpfValidator>();
builder.Services.AddScoped<IScoreCalculator, ScoreCalculator>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await initializer.InitializeAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
