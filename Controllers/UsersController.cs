using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using FitAI.Models;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FitAI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IConfiguration configuration, ILogger<UsersController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        // Метод для хеширования пароля
        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        // Регистрация пользователя
        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            _logger.LogInformation("Начало регистрации пользователя.");
            using var connection = new MySqlConnection(_connectionString);
            try
            {
                await connection.OpenAsync();
                _logger.LogInformation("Подключение к базе данных успешно открыто.");

                // Проверяем, существует ли пользователь
                var checkQuery = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
                using var checkCommand = new MySqlCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@Email", user.Email);
                var exists = (long)await checkCommand.ExecuteScalarAsync();

                if (exists > 0)
                {
                    _logger.LogWarning("Попытка регистрации существующего пользователя: {Email}", user.Email);
                    return BadRequest(new { message = "Пользователь с таким Email уже существует." });
                }

                // Хешируем пароль
                string hashedPassword = HashPassword(user.Password);

                // Вставляем пользователя
                var insertQuery = "INSERT INTO Users (Password, Email) VALUES (@Password, @Email)";
                using var command = new MySqlCommand(insertQuery, connection);
                command.Parameters.AddWithValue("@Password", hashedPassword);
                command.Parameters.AddWithValue("@Email", user.Email);
                await command.ExecuteNonQueryAsync();

                _logger.LogInformation("Пользователь {Email} успешно зарегистрирован.", user.Email);
                return Ok(new { message = "Регистрация прошла успешно." });
            }
            catch (MySqlException ex)
            {
                _logger.LogError(ex, "Ошибка при регистрации пользователя {Email}.", user.Email);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ошибка при регистрации." });
            }
        }

        // Аутентификация пользователя
        [HttpPost("login")]
        public async Task<IActionResult> Login(User user)
        {
            _logger.LogInformation("Попытка входа пользователя: {Email}", user.Email);
            using var connection = new MySqlConnection(_connectionString);
            try
            {
                await connection.OpenAsync();
                _logger.LogInformation("Подключение к базе данных успешно открыто.");

                // Запрос только пароля (улучшает безопасность)
                var command = new MySqlCommand("SELECT UserID, Password FROM Users WHERE Email = @Email", connection);
                command.Parameters.AddWithValue("@Email", user.Email);
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    string storedPassword = reader.GetString(reader.GetOrdinal("Password"));
                    if (storedPassword == HashPassword(user.Password))
                    {
                        int userId = reader.GetInt32(reader.GetOrdinal("UserID"));
                        _logger.LogInformation("Пользователь {Email} успешно вошел в систему.", user.Email);
                        return Ok(new { UserID = userId, Email = user.Email });
                    }
                }

                _logger.LogWarning("Неудачная попытка входа: {Email}", user.Email);
                return Unauthorized(new { message = "Неправильный Email или пароль." });
            }
            catch (MySqlException ex)
            {
                _logger.LogError(ex, "Ошибка входа пользователя {Email}.", user.Email);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ошибка при попытке входа." });
            }
        }
    }
}
