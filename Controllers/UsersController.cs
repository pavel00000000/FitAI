using FitAI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using FitAI.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System;
using System.Text;

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

        // Модель для регистрации
        public class RegisterModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string ConfirmPassword { get; set; }
        }

        // Регистрация пользователя
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            _logger.LogInformation("Начало регистрации пользователя.");

            if (model.Password != model.ConfirmPassword)
            {
                return BadRequest(new { message = "Пароли не совпадают." });
            }

            try
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (existingUser != null)
                {
                    _logger.LogWarning("Попытка регистрации существующего пользователя: {Email}", model.Email);
                    return BadRequest(new { message = "Пользователь с таким Email уже существует." });
                }

                string hashedPassword = HashPassword(model.Password);

                var user = new User { Email = model.Email, Password = hashedPassword };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Пользователь {Email} успешно зарегистрирован.", model.Email);
                return Ok(new { message = "Регистрация прошла успешно." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при регистрации пользователя {Email}.", model.Email);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ошибка при регистрации." });
            }
        }

        // Аутентификация пользователя
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            _logger.LogInformation("Попытка входа пользователя: {Email}", user.Email);

            try
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == user.Email);

                if (existingUser == null || !VerifyPassword(user.Password, existingUser.Password))
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

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            string enteredHash = HashPassword(enteredPassword);
            return enteredHash == storedHash;
        }
    }
}
