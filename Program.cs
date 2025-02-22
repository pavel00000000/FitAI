using FitAI.Data;
using Microsoft.EntityFrameworkCore;

namespace FitAI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Добавляем конфигурацию из appsettings.json
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            // Подключаем базу данных SQLite
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Настраиваем контроллеры и JSON-сериализацию
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                    options.JsonSerializerOptions.PropertyNamingPolicy = null; // Сохраняем имена свойств как есть
                });

            // Добавляем Swagger для документации API
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "FitAI API", Version = "v1" });
            });

            // Настраиваем CORS для разрешения запросов с любого origin (только для тестов, не для продакшена!)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost", builder =>
                {
                    builder.AllowAnyOrigin() // Разрешаем все origin'ы (только для тестов, не для продакшена!)
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            // Настраиваем логирование
            builder.Services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            });

            var app = builder.Build();

            // Настраиваем middleware
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();
            app.UseHttpsRedirection();

            // Включаем CORS
            app.UseCors("AllowLocalhost");

            // Убираем UseAuthentication и UseAuthorization, так как нет токенов
            app.MapControllers();

            app.Run();
        }
    }
}