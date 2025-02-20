using FitAI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using FitAI.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;

namespace FitAI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfilesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserProfilesController> _logger;

        public UserProfilesController(AppDbContext context, ILogger<UserProfilesController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateUserProfile([FromBody] UserProfileRequest profile)
        {
            _logger.LogInformation("Создание профиля для пользователя {UserID}", profile.UserID);

            var userExists = await _context.Users.AnyAsync(u => u.UserID == profile.UserID);
            if (!userExists)
            {
                return BadRequest(new { message = "Пользователь не найден." });
            }

            var existingProfile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == profile.UserID);
            if (existingProfile != null)
            {
                return BadRequest(new { message = "Профиль для этого пользователя уже существует. Используйте метод обновления." });
            }

            var userProfile = new UserProfile
            {
                UserID = profile.UserID,
                FullName = profile.FullName,
                BodyType = profile.BodyType,
                Height = profile.Height,
                Weight = profile.Weight,
                Age = profile.Age,
                Sex = profile.Sex,
                MainGoals = profile.MainGoals,
                LevelOfPhysicalFitness = profile.LevelOfPhysicalFitness
            };

            _context.UserProfiles.Add(userProfile);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Профиль успешно создан." });
        }

        // Временная модель для запроса, чтобы Swagger корректно отображал обёртку
        public class UserProfileRequest
        {
            public int UserID { get; set; }
            public string FullName { get; set; }
            public string BodyType { get; set; }
            public int? Height { get; set; }
            public int? Weight { get; set; }
            public int? Age { get; set; }
            public Gender? Sex { get; set; }
            public string MainGoals { get; set; }
            public PhysicalFitnessLevel? LevelOfPhysicalFitness { get; set; }
        }

        [HttpPut("update/{userId}")]
        public async Task<IActionResult> UpdateUserProfile(int userId, [FromBody] UserProfile profileUpdate)
        {
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);
            if (profile == null)
            {
                return NotFound(new { message = "Профиль не найден. Сначала создайте профиль." });
            }

            // Обновляем только те поля, которые переданы
            profile.FullName = profileUpdate.FullName ?? profile.FullName;
            profile.BodyType = profileUpdate.BodyType ?? profile.BodyType;
            profile.Height = profileUpdate.Height ?? profile.Height;
            profile.Weight = profileUpdate.Weight ?? profile.Weight;
            profile.Age = profileUpdate.Age ?? profile.Age;
            profile.Sex = profileUpdate.Sex ?? profile.Sex;
            profile.MainGoals = profileUpdate.MainGoals ?? profile.MainGoals;
            profile.LevelOfPhysicalFitness = profileUpdate.LevelOfPhysicalFitness ?? profile.LevelOfPhysicalFitness;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Профиль успешно обновлён." });
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserProfile(int userId)
        {
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);
            if (profile == null)
            {
                return NotFound(new { message = "Профиль не найден." });
            }
            return Ok(profile);
        }

        //[HttpPut("update/{userId}")]
        //public async Task<IActionResult> UpdateUserProfile(int userId, [FromBody] UserProfile profileUpdate)
        //{
        //    var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);
        //    if (profile == null)
        //    {
        //        return NotFound(new { message = "Профиль не найден." });
        //    }

        //    profile.FullName = profileUpdate.FullName;
        //    profile.BodyType = profileUpdate.BodyType;
        //    profile.Height = profileUpdate.Height;
        //    profile.Weight = profileUpdate.Weight;
        //    profile.Age = profileUpdate.Age;
        //    profile.Sex = profileUpdate.Sex;
        //    profile.MainGoals = profileUpdate.MainGoals;
        //    profile.LevelOfPhysicalFitness = profileUpdate.LevelOfPhysicalFitness;

        //    await _context.SaveChangesAsync();
        //    return Ok(new { message = "Профиль успешно обновлён." });
        //}

        [HttpDelete("delete/{userId}")]
        public async Task<IActionResult> DeleteUserProfile(int userId)
        {
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);
            if (profile == null)
            {
                return NotFound(new { message = "Профиль не найден." });
            }

            _context.UserProfiles.Remove(profile);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Профиль удалён." });
        }
    }
}
