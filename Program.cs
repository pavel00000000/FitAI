using FitAI.Data;
using Microsoft.EntityFrameworkCore;

namespace FitAI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ��������� ������������ �� appsettings.json
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            // ��������� ������� � ���������
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))); // ���������� SQLite

            builder.Services.AddControllers(); // ���������� �����������
            builder.Services.AddEndpointsApiExplorer(); // ���������� ������������ API
            builder.Services.AddSwaggerGen(); // ��������� Swagger

            // ����������� �����������
            builder.Services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            });

            var app = builder.Build();

            // �������� Swagger ��� ����������
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
