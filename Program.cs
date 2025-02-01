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

            // Добавляем сервисы в контейнер
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))); // Используем SQLite

            builder.Services.AddControllers(); // Подключаем контроллеры
            builder.Services.AddEndpointsApiExplorer(); // Подключаем документацию API
            builder.Services.AddSwaggerGen(); // Добавляем Swagger

            // Настраиваем логирование
            builder.Services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            });

            var app = builder.Build();

            // Включаем Swagger при разработке
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
