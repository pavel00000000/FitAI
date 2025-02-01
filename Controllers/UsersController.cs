using FitAI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using FitAI.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FitAI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UsersController> _logger;

        public UsersController(AppDbContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Регистрация пользователя
        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            _logger.LogInformation("Начало регистрации пользователя.");

            try
            {
                // Проверяем, существует ли пользователь
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == user.Email);

                if (existingUser != null)
                {
                    _logger.LogWarning("Попытка регистрации существующего пользователя: {Email}", user.Email);
                    return BadRequest(new { message = "Пользователь с таким Email уже существует." });
                }

                // Добавляем пользователя в базу данных
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Пользователь {Email} успешно зарегистрирован.", user.Email);
                return Ok(new { message = "Регистрация прошла успешно." });
            }
            catch (Exception ex)
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

            try
            {
                // Поиск пользователя по email
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == user.Email);

                if (existingUser == null || existingUser.Password != user.Password) // Без хеширования
                {
                    _logger.LogWarning("Неудачная попытка входа: {Email}", user.Email);
                    return Unauthorized(new { message = "Неправильный Email или пароль." });
                }

                _logger.LogInformation("Пользователь {Email} успешно вошел в систему.", user.Email);
                return Ok(new { UserID = existingUser.UserID, Email = existingUser.Email });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка входа пользователя {Email}.", user.Email);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ошибка при попытке входа." });
            }
        }
    }
}
