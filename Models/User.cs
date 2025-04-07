using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FitAI.Models
{
    public class User
    {
        public int UserID { get; set; }

        [Required(ErrorMessage = "Email обязателен.")]
        [EmailAddress(ErrorMessage = "Некорректный формат Email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль обязателен.")]
        [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов.")]
        public string Password { get; set; }

        
        public virtual UserProfile UserProfile { get; set; }
    }

    public class UserProfile
    {
        [Key, ForeignKey("User")]
        public int UserID { get; set; }

        public virtual User User { get; set; } 

        [StringLength(100, ErrorMessage = "Имя не должно превышать 100 символов.")]
        public string FullName { get; set; }

        public string BodyType { get; set; }

        [Range(50, 300, ErrorMessage = "Рост должен быть в диапазоне от 50 до 300 см.")]
        public int? Height { get; set; }

        [Range(20, 500, ErrorMessage = "Вес должен быть в диапазоне от 20 до 500 кг.")]
        public int? Weight { get; set; }

        [Range(1, 120, ErrorMessage = "Возраст должен быть в диапазоне от 1 до 120 лет.")]
        public int? Age { get; set; }

        public Gender? Sex { get; set; }

        public string MainGoals { get; set; }

        public PhysicalFitnessLevel? LevelOfPhysicalFitness { get; set; }
    }

    public enum Gender
    {
        Male,
        Female
    }

    public enum PhysicalFitnessLevel
    {
        Beginner,
        Intermediate,
        Advanced
    }

    public class WorkoutPlan
    {
        [Key]
        public int WorkoutPlanId { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }

        [Required(ErrorMessage = "Название плана обязательно.")]
        [StringLength(100, ErrorMessage = "Название не должно превышать 100 символов.")]
        public string PlanName { get; set; }

        [Required(ErrorMessage = "Описание плана обязательно.")]
        public string Description { get; set; }

        public PhysicalFitnessLevel LevelOfPhysicalFitness { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        
        [JsonIgnore]
        public virtual User User { get; set; }

        public virtual ICollection<WorkoutExercise> Exercises { get; set; } = new List<WorkoutExercise>();
    }

    public class WorkoutExercise
    {
        [Key]
        public int WorkoutExerciseId { get; set; }

        [ForeignKey("WorkoutPlan")]
        public int WorkoutPlanId { get; set; }

        [Required(ErrorMessage = "Название упражнения обязательно.")]
        [StringLength(100, ErrorMessage = "Название упражнения не должно превышать 100 символов.")]
        public string ExerciseName { get; set; }

        [Required(ErrorMessage = "Тип упражнения обязателен.")]
        public string ExerciseType { get; set; } 

        [Range(0, 1000, ErrorMessage = "Вес должен быть в диапазоне от 0 до 1000 кг.")]
        public int? Weight { get; set; }

        [Range(1, 50, ErrorMessage = "Количество повторений должно быть от 1 до 50.")]
        public int? Repetitions { get; set; }

        [Range(1, 10, ErrorMessage = "Количество подходов должно быть от 1 до 10.")]
        public int? Approaches { get; set; }

        
        [JsonIgnore]
        public virtual WorkoutPlan WorkoutPlan { get; set; }
    }

    public class WorkoutPlanDto
    {
        public int WorkoutPlanId { get; set; }
        public int UserID { get; set; }
        public string PlanName { get; set; }
        public string Description { get; set; }
        public PhysicalFitnessLevel LevelOfPhysicalFitness { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<WorkoutExerciseDto> Exercises { get; set; } = new List<WorkoutExerciseDto>();
    }

    public class WorkoutExerciseDto
    {
        public int WorkoutExerciseId { get; set; }
        public int WorkoutPlanId { get; set; }
        public string ExerciseName { get; set; }
        public string ExerciseType { get; set; }
        public int? Weight { get; set; }
        public int? Repetitions { get; set; }
        public int? Approaches { get; set; }
    }
}