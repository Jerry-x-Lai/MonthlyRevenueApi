
using MediatR;
using AutoMapper;
using MonthlyRevenueApi.Features;
using MonthlyRevenueApi.Infrastructure.Database;
using MonthlyRevenueApi.Services;

var builder = WebApplication.CreateBuilder(args);

// DbConnectionFactory & SqlExtension DI
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<SqlExtension>();

// Service DI
builder.Services.AddScoped<IMonthlyRevenueService, MonthlyRevenueService>();

// Add services to the container.

builder.Services.AddControllers(options =>
{
	options.Filters.Add<MonthlyRevenueApi.Filters.GlobalExceptionFilter>();
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// MediatR
builder.Services.AddMediatR(typeof(Program));

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.

// Always enable Swagger for demo
app.UseSwagger();
app.UseSwaggerUI();


app.UseAuthorization();

app.MapControllers();

app.Run();
