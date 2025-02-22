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

            // ���������� ���� ������ SQLite
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ����������� ����������� � JSON-������������
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                    options.JsonSerializerOptions.PropertyNamingPolicy = null; // ��������� ����� ������� ��� ����
                });

            // ��������� Swagger ��� ������������ API
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "FitAI API", Version = "v1" });
            });

            // ����������� CORS ��� ���������� �������� � ������ origin (������ ��� ������, �� ��� ����������!)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost", builder =>
                {
                    builder.AllowAnyOrigin() // ��������� ��� origin'� (������ ��� ������, �� ��� ����������!)
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            // ����������� �����������
            builder.Services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            });

            var app = builder.Build();

            // ����������� middleware
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();
            app.UseHttpsRedirection();

            // �������� CORS
            app.UseCors("AllowLocalhost");

            // ������� UseAuthentication � UseAuthorization, ��� ��� ��� �������
            app.MapControllers();

            app.Run();
        }
    }
}