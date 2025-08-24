
using MediatR;
using MonthlyRevenueApi.Features;
using MonthlyRevenueApi.Infrastructure.Database;
using MonthlyRevenueApi.Services;


var builder = WebApplication.CreateBuilder(args);

// 讀取自訂環境設計檔
var envConfig = new ConfigurationBuilder()
	.SetBasePath(Directory.GetCurrentDirectory())
	.AddJsonFile("appsettings.env.json", optional: true, reloadOnChange: true)
	.Build();

var envName = builder.Environment.EnvironmentName;
var frontendUrl = envConfig[$"Environments:{envName}:FrontendUrl"] ?? "http://localhost:3000";

// 註冊 CORS
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowFrontend", policy =>
	{
		policy.WithOrigins(frontendUrl)
			  .AllowAnyHeader()
			  .AllowAnyMethod();
	});
});

// DbConnectionFactory & SqlExtension DI
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<SqlExtension>();

// Service DI
builder.Services.AddScoped<IMonthlyRevenueService, MonthlyRevenueService>();

// Add services to the container.

builder.Services.AddControllers(options =>
{
	options.Filters.Add<MonthlyRevenueApi.Filters.GlobalExceptionFilter>();
})
.AddJsonOptions(options =>
{
	options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// MediatR
builder.Services.AddMediatR(typeof(Program));

// AutoMapper
builder.Services.AddAutoMapper(typeof(ImportMonthlyRevenueProfile).Assembly);


var app = builder.Build();

// Configure the HTTP request pipeline.


// 僅開發環境啟用 Swagger
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

// 啟用 CORS
app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
