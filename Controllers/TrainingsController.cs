using FitAI.Data;
using FitAI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FitAI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkoutController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<WorkoutController> _logger;

        public WorkoutController(AppDbContext context, ILogger<WorkoutController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Модель для ввода данных пользователем для генерации плана
        public class GenerateWorkoutModel
        {
            public string Goals { get; set; }
            public string Preferences { get; set; }
            public bool HasEquipment { get; set; }
            public string Restrictions { get; set; }
            public PhysicalFitnessLevel? LevelOfPhysicalFitness { get; set; }
        }

        // Модель для редактирования упражнения
        public class EditExerciseModel
        {
            public int WorkoutExerciseId { get; set; }
            public string ExerciseName { get; set; }
            public string ExerciseType { get; set; }
            public int? Weight { get; set; }
            public int? Repetitions { get; set; }
            public int? Approaches { get; set; }
        }

        // Модель для ввода показателей силы (первый скриншот)
        public class PowerIndicatorInputModel
        {
            public int UserId { get; set; }
            public PowerIndicatorExercise Deadlift { get; set; }
            public PowerIndicatorExercise LegPress { get; set; }
            public PowerIndicatorExercise BenchPress { get; set; }
        }

        public class PowerIndicatorExercise
        {
            public int? Weight { get; set; }
            public int? Repetitions { get; set; }
            public int? Approaches { get; set; }
        }

        // Модель для изменения веса
        public class UpdateWeightModel
        {
            public int UserId { get; set; }
            public string ExerciseName { get; set; }
            public int WeightChange { get; set; }
        }

        // Получение готовых планов
        [HttpGet("ready-plans")]
        public async Task<IActionResult> GetReadyPlans()
        {
            _logger.LogInformation("Запрос готовых тренировочных планов.");

            try
            {
                var readyPlans = new List<WorkoutPlan>
                {
                    new WorkoutPlan
                    {
                        PlanName = "Для новичка 3 раза в неделю",
                        Description = "Базовая программа для начинающих с акцентом на основные упражнения.",
                        LevelOfPhysicalFitness = PhysicalFitnessLevel.Beginner
                    },
                    new WorkoutPlan
                    {
                        PlanName = "Силовая программа для продвинутых",
                        Description = "Интенсивные тренировки для увеличения силы и мышечной массы.",
                        LevelOfPhysicalFitness = PhysicalFitnessLevel.Advanced
                    },
                    new WorkoutPlan
                    {
                        PlanName = "Функциональный тренинг",
                        Description = "Программа для улучшения общей физической подготовки.",
                        LevelOfPhysicalFitness = PhysicalFitnessLevel.Intermediate
                    }
                };

                _logger.LogInformation("Готовые планы успешно возвращены.");
                return Ok(readyPlans);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении готовых планов.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ошибка при получении готовых планов." });
            }
        }

        // Генерация персонализированного плана
        [HttpPost("generate-plan")]
        public async Task<IActionResult> GenerateWorkoutPlan(int userId, [FromBody] GenerateWorkoutModel model)
        {
            _logger.LogInformation("Попытка генерации персонализированного плана для пользователя с ID: {UserID}", userId);

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

                // Логика генерации плана (упрощённая версия, можно дополнить ИИ-алгоритмами)
                var workoutPlan = new WorkoutPlan
                {
                    UserID = userId,
                    PlanName = $"Персонализированный план для {user.UserProfile.FullName}",
                    Description = $"Сгенерированный план на основе целей: {model.Goals}, предпочтений: {model.Preferences}, ограничений: {model.Restrictions}",
                    LevelOfPhysicalFitness = model.LevelOfPhysicalFitness ?? user.UserProfile.LevelOfPhysicalFitness ?? PhysicalFitnessLevel.Beginner
                };

                // Добавляем базовые упражнения (пример)
                workoutPlan.Exercises = new List<WorkoutExercise>
                {
                    new WorkoutExercise
                    {
                        ExerciseName = "Приседания",
                        ExerciseType = "Leg Press",
                        Weight = 50,
                        Repetitions = 10,
                        Approaches = 3
                    },
                    new WorkoutExercise
                    {
                        ExerciseName = "Жим лежа",
                        ExerciseType = "Bench Press",
                        Weight = 40,
                        Repetitions = 8,
                        Approaches = 4
                    }
                };

                _context.WorkoutPlans.Add(workoutPlan);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Персонализированный план успешно сгенерирован для пользователя с ID {UserID}.", userId);
                return Ok(new { message = "План успешно сгенерирован.", WorkoutPlanId = workoutPlan.WorkoutPlanId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при генерации плана для пользователя с ID {UserID}.", userId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ошибка при генерации плана." });
            }
        }

        // Получение плана пользователя
        [HttpGet("user-plan/{userId}")]
        public async Task<IActionResult> GetUserWorkoutPlan(int userId)
        {
            _logger.LogInformation("Запрос плана тренировок для пользователя с ID: {UserID}", userId);

            try
            {
                var workoutPlan = await _context.WorkoutPlans
                    .Include(wp => wp.Exercises)
                    .FirstOrDefaultAsync(wp => wp.UserID == userId);

                if (workoutPlan == null)
                {
                    _logger.LogWarning("План тренировок для пользователя с ID {UserID} не найден.", userId);
                    return NotFound(new { message = "План тренировок не найден." });
                }

                var workoutPlanDto = new WorkoutPlanDto
                {
                    WorkoutPlanId = workoutPlan.WorkoutPlanId,
                    UserID = workoutPlan.UserID,
                    PlanName = workoutPlan.PlanName,
                    Description = workoutPlan.Description,
                    LevelOfPhysicalFitness = workoutPlan.LevelOfPhysicalFitness,
                    CreatedDate = workoutPlan.CreatedDate,
                    Exercises = workoutPlan.Exercises.Select(e => new WorkoutExerciseDto
                    {
                        WorkoutExerciseId = e.WorkoutExerciseId,
                        WorkoutPlanId = e.WorkoutPlanId,
                        ExerciseName = e.ExerciseName,
                        ExerciseType = e.ExerciseType,
                        Weight = e.Weight,
                        Repetitions = e.Repetitions,
                        Approaches = e.Approaches
                    }).ToList()
                };

                _logger.LogInformation("План тренировок успешно возвращен для пользователя с ID {UserID}.", userId);
                return Ok(workoutPlanDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении плана для пользователя с ID {UserID}.", userId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ошибка при получении плана." });
            }
        }

        // Получение показателей силы (второй скриншот)
        [HttpGet("power-indicators/{userId}")]
        public async Task<IActionResult> GetPowerIndicators(int userId)
        {
            _logger.LogInformation("Запрос показателей силы для пользователя с ID: {UserID}", userId);

            try
            {
                var workoutPlan = await _context.WorkoutPlans
                    .Include(wp => wp.Exercises)
                    .FirstOrDefaultAsync(wp => wp.UserID == userId);

                if (workoutPlan == null)
                {
                    _logger.LogWarning("План тренировок для пользователя с ID {UserID} не найден.", userId);
                    return NotFound(new { message = "План тренировок не найден." });
                }

                var powerIndicators = workoutPlan.Exercises
                    .Where(e => e.ExerciseName == "Deadlift" || e.ExerciseName == "Leg Press" || e.ExerciseName == "Bench Press")
                    .Select(e => new
                    {
                        ExerciseName = e.ExerciseName,
                        Weight = e.Weight,
                        Repetitions = e.Repetitions,
                        Approaches = e.Approaches
                    })
                    .ToList();

                _logger.LogInformation("Показатели силы успешно возвращены для пользователя с ID {UserID}.", userId);
                return Ok(powerIndicators);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении показателей силы для пользователя с ID {UserID}.", userId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ошибка при получении показателей силы." });
            }
        }

        // Сохранение показателей силы (первый скриншот)
        [HttpPost("save-power-indicators")]
        public async Task<IActionResult> SavePowerIndicators([FromBody] PowerIndicatorInputModel model)
        {
            _logger.LogInformation("Попытка сохранения показателей силы для пользователя с ID: {UserID}", model.UserId);

            try
            {
                var existingPlan = await _context.WorkoutPlans
                    .Include(wp => wp.Exercises)
                    .FirstOrDefaultAsync(wp => wp.UserID == model.UserId);

                if (existingPlan == null)
                {
                    existingPlan = new WorkoutPlan
                    {
                        UserID = model.UserId,
                        PlanName = "Power Indicators Plan",
                        Description = "План для отслеживания показателей силы",
                        LevelOfPhysicalFitness = PhysicalFitnessLevel.Beginner,
                        CreatedDate = DateTime.UtcNow
                    };
                    _context.WorkoutPlans.Add(existingPlan);
                    await _context.SaveChangesAsync();
                }

                var existingExercises = existingPlan.Exercises
                    .Where(e => e.ExerciseName == "Deadlift" || e.ExerciseName == "Leg Press" || e.ExerciseName == "Bench Press")
                    .ToList();
                _context.WorkoutExercises.RemoveRange(existingExercises);

                var exercisesToAdd = new List<WorkoutExercise>();

                if (model.Deadlift != null)
                {
                    exercisesToAdd.Add(new WorkoutExercise
                    {
                        WorkoutPlanId = existingPlan.WorkoutPlanId,
                        ExerciseName = "Deadlift",
                        ExerciseType = "Strength",
                        Weight = model.Deadlift.Weight,
                        Repetitions = model.Deadlift.Repetitions,
                        Approaches = model.Deadlift.Approaches
                    });
                }

                if (model.LegPress != null)
                {
                    exercisesToAdd.Add(new WorkoutExercise
                    {
                        WorkoutPlanId = existingPlan.WorkoutPlanId,
                        ExerciseName = "Leg Press",
                        ExerciseType = "Strength",
                        Weight = model.LegPress.Weight,
                        Repetitions = model.LegPress.Repetitions,
                        Approaches = model.LegPress.Approaches
                    });
                }

                if (model.BenchPress != null)
                {
                    exercisesToAdd.Add(new WorkoutExercise
                    {
                        WorkoutPlanId = existingPlan.WorkoutPlanId,
                        ExerciseName = "Bench Press",
                        ExerciseType = "Strength",
                        Weight = model.BenchPress.Weight,
                        Repetitions = model.BenchPress.Repetitions,
                        Approaches = model.BenchPress.Approaches
                    });
                }

                _context.WorkoutExercises.AddRange(exercisesToAdd);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Показатели силы успешно сохранены для пользователя с ID {UserID}.", model.UserId);
                return Ok(new { message = "Показатели силы успешно сохранены.", WorkoutPlanId = existingPlan.WorkoutPlanId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при сохранении показателей силы для пользователя с ID {UserID}.", model.UserId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ошибка при сохранении показателей силы." });
            }
        }

        // Изменение веса (+5 кг или -10 кг)
        [HttpPut("update-weight")]
        public async Task<IActionResult> UpdateWeight([FromBody] UpdateWeightModel model)
        {
            _logger.LogInformation("Попытка изменения веса для упражнения {ExerciseName} пользователя с ID: {UserID}", model.ExerciseName, model.UserId);

            try
            {
                var workoutPlan = await _context.WorkoutPlans
                    .Include(wp => wp.Exercises)
                    .FirstOrDefaultAsync(wp => wp.UserID == model.UserId);

                if (workoutPlan == null)
                {
                    _logger.LogWarning("План тренировок для пользователя с ID {UserID} не найден.", model.UserId);
                    return NotFound(new { message = "План тренировок не найден." });
                }

                var exercise = workoutPlan.Exercises
                    .FirstOrDefault(e => e.ExerciseName == model.ExerciseName);

                if (exercise == null)
                {
                    _logger.LogWarning("Упражнение {ExerciseName} не найдено в плане пользователя с ID {UserID}.", model.ExerciseName, model.UserId);
                    return NotFound(new { message = "Упражнение не найдено." });
                }

                exercise.Weight = (exercise.Weight ?? 0) + model.WeightChange;
                if (exercise.Weight < 0)
                {
                    exercise.Weight = 0;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Вес для упражнения {ExerciseName} успешно обновлен для пользователя с ID {UserID}.", model.ExerciseName, model.UserId);
                return Ok(new { message = "Вес успешно обновлен.", NewWeight = exercise.Weight });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении веса для упражнения {ExerciseName} пользователя с ID {UserID}.", model.ExerciseName, model.UserId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ошибка при обновлении веса." });
            }
        }

        // Редактирование упражнения в плане
        [HttpPut("edit-exercise")]
        public async Task<IActionResult> EditExercise(int userId, [FromBody] EditExerciseModel model)
        {
            _logger.LogInformation("Попытка редактирования упражнения для пользователя с ID: {UserID}", userId);

            try
            {
                var workoutPlan = await _context.WorkoutPlans
                    .Include(wp => wp.Exercises)
                    .FirstOrDefaultAsync(wp => wp.UserID == userId);

                if (workoutPlan == null)
                {
                    _logger.LogWarning("План тренировок для пользователя с ID {UserID} не найден.", userId);
                    return NotFound(new { message = "План тренировок не найден." });
                }

                var exercise = workoutPlan.Exercises.FirstOrDefault(e => e.WorkoutExerciseId == model.WorkoutExerciseId);
                if (exercise == null)
                {
                    _logger.LogWarning("Упражнение с ID {WorkoutExerciseId} не найдено в плане пользователя с ID {UserID}.", model.WorkoutExerciseId, userId);
                    return NotFound(new { message = "Упражнение не найдено." });
                }

                exercise.ExerciseName = model.ExerciseName ?? exercise.ExerciseName;
                exercise.ExerciseType = model.ExerciseType ?? exercise.ExerciseType;
                exercise.Weight = model.Weight ?? exercise.Weight;
                exercise.Repetitions = model.Repetitions ?? exercise.Repetitions;
                exercise.Approaches = model.Approaches ?? exercise.Approaches;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Упражнение успешно отредактировано для пользователя с ID {UserID}.", userId);
                return Ok(new { message = "Упражнение успешно отредактировано.", WorkoutExerciseId = exercise.WorkoutExerciseId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при редактировании упражнения для пользователя с ID {UserID}.", userId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ошибка при редактировании упражнения." });
            }
        }

        [HttpPost("save-plan/{userId}")]
        public async Task<IActionResult> SaveWorkoutPlan(int userId, [FromBody] WorkoutPlanDto updatedPlan)
        {
            _logger.LogInformation("Попытка сохранения плана для пользователя с ID: {UserID}", userId);

            try
            {
                var existingPlan = await _context.WorkoutPlans
                    .Include(wp => wp.Exercises)
                    .FirstOrDefaultAsync(wp => wp.UserID == userId && wp.WorkoutPlanId == updatedPlan.WorkoutPlanId);

                if (existingPlan == null)
                {
                    existingPlan = new WorkoutPlan
                    {
                        UserID = userId,
                        PlanName = updatedPlan.PlanName,
                        Description = updatedPlan.Description,
                        LevelOfPhysicalFitness = updatedPlan.LevelOfPhysicalFitness,
                        CreatedDate = updatedPlan.CreatedDate
                    };
                    _context.WorkoutPlans.Add(existingPlan);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    existingPlan.PlanName = updatedPlan.PlanName;
                    existingPlan.Description = updatedPlan.Description;
                    existingPlan.LevelOfPhysicalFitness = updatedPlan.LevelOfPhysicalFitness;
                }

                foreach (var exerciseDto in updatedPlan.Exercises)
                {
                    if (exerciseDto.WorkoutExerciseId == 0)
                    {
                        var newExercise = new WorkoutExercise
                        {
                            WorkoutPlanId = existingPlan.WorkoutPlanId,
                            ExerciseName = exerciseDto.ExerciseName,
                            ExerciseType = exerciseDto.ExerciseType,
                            Weight = exerciseDto.Weight,
                            Repetitions = exerciseDto.Repetitions,
                            Approaches = exerciseDto.Approaches
                        };
                        _context.WorkoutExercises.Add(newExercise);
                    }
                    else
                    {
                        var existingExercise = existingPlan.Exercises
                            .FirstOrDefault(e => e.WorkoutExerciseId == exerciseDto.WorkoutExerciseId);
                        if (existingExercise != null)
                        {
                            existingExercise.ExerciseName = exerciseDto.ExerciseName;
                            existingExercise.ExerciseType = exerciseDto.ExerciseType;
                            existingExercise.Weight = exerciseDto.Weight;
                            existingExercise.Repetitions = exerciseDto.Repetitions;
                            existingExercise.Approaches = exerciseDto.Approaches;
                        }
                    }
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("План успешно сохранен для пользователя с ID {UserID}.", userId);
                return Ok(new { message = "План успешно сохранен.", WorkoutPlanId = existingPlan.WorkoutPlanId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при сохранении плана для пользователя с ID {UserID}.", userId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ошибка при сохранении плана." });
            }
        }
    }
}


