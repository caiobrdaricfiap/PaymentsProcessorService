using Prometheus;
using FiapCloudGames.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using PaymentsProcessorService.Api.IoC;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDependencyInjection();

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// HTTP Metrics (Prometheus)
app.UseHttpMetrics();

app.UseAuthorization();

app.MapControllers();

// Endpoint /metrics (Prometheus)
app.MapMetrics();

app.Run();
