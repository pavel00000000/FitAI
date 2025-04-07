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
using System.ComponentModel.DataAnnotations;

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
        public class LoginModel
        {
            [Required(ErrorMessage = "Email обязателен.")]
            [EmailAddress(ErrorMessage = "Некорректный формат Email.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Пароль обязателен.")]
            [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов.")]
            public string Password { get; set; }
        }

        // Модель для регистрации
        public class RegisterModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string ConfirmPassword { get; set; }
        }
        // Модель для профиля пользователя
        public class UserProfileModel
        {
            public string FullName { get; set; }
            public string BodyType { get; set; }
            public int? Height { get; set; }
            public int? Weight { get; set; }
            public int? Age { get; set; }
            public Gender? Sex { get; set; }
            public string MainGoals { get; set; }
            public PhysicalFitnessLevel? LevelOfPhysicalFitness { get; set; }
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
                return Ok(new { message = "Регистрация прошла успешно.", UserID = user.UserID });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при регистрации пользователя {Email}.", model.Email);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ошибка при регистрации." });
            }
        }
        // Аутентификация пользователя
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            _logger.LogInformation("Попытка входа пользователя: {Email}", model.Email);

            try
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (existingUser == null || !VerifyPassword(model.Password, existingUser.Password))
                {
                    _logger.LogWarning("Неудачная попытка входа: {Email}", model.Email);
                    return Unauthorized(new { message = "Неправильный Email или пароль." });
                }

                if (existingUser.UserID <= 0)
                {
                    _logger.LogError("Некорректный UserID для пользователя {Email}: {UserID}", model.Email, existingUser.UserID);
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ошибка: некорректный идентификатор пользователя." });
                }

                _logger.LogInformation("Пользователь {Email} успешно вошел в систему. UserID: {UserID}", model.Email, existingUser.UserID);
                return Ok(new { userId = existingUser.UserID, email = existingUser.Email });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка входа пользователя {Email}.", model.Email);
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
       
        // Создание/обновление профиля
        [HttpPost("profile")]
        public async Task<IActionResult> UpdateProfile(int userId, [FromBody] UserProfileModel model)
        {
            _logger.LogInformation("Попытка обновления профиля для пользователя с ID: {UserID}", userId);

            try
            {
                var user = await _context.Users
                    .Include(u => u.UserProfile) 
                    .FirstOrDefaultAsync(u => u.UserID == userId);

                if (user == null)
                {
                    _logger.LogWarning("Пользователь с ID {UserID} не найден.", userId);
                    return NotFound(new { message = "Пользователь не найден." });
                }

                
                if (user.UserProfile == null)
                {
                    user.UserProfile = new UserProfile { UserID = userId };
                }

                
                user.UserProfile.FullName = model.FullName;
                user.UserProfile.BodyType = model.BodyType;
                user.UserProfile.Height = model.Height;
                user.UserProfile.Weight = model.Weight; 
                user.UserProfile.Age = model.Age;
                user.UserProfile.Sex = model.Sex;
                user.UserProfile.MainGoals = model.MainGoals;
                user.UserProfile.LevelOfPhysicalFitness = model.LevelOfPhysicalFitness;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Профиль пользователя с ID {UserID} успешно обновлен.", userId);
                return Ok(new { message = "Профиль успешно обновлен.", UserID = userId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении профиля пользователя с ID {UserID}.", userId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ошибка при обновлении профиля." });
            }
        }
    }
}
