using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        // Навигационное свойство для профиля
        public virtual UserProfile UserProfile { get; set; }
    }

    public class UserProfile
    {
        [Key, ForeignKey("User")]
        public int UserID { get; set; }

        public virtual User User { get; set; } // Навигационное свойство

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
}