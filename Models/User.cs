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

        public UserProfile UserProfile { get; set; }
    }

    public class UserProfile
    {
        [Key, ForeignKey("User")]
        public int UserID { get; set; }

        public User User { get; set; } // Навигационное свойство, не требуется в запросе

        [Required(ErrorMessage = "Полное имя обязательно.")]
        public string FullName { get; set; } // Обязательное

        [Required(ErrorMessage = "Тип тела обязателен.")]
        public string BodyType { get; set; } // Обязательное

        [Required(ErrorMessage = "Рост обязателен.")]
        public int? Height { get; set; } // Обязательное

        [Required(ErrorMessage = "Вес обязателен.")]
        public int? Weight { get; set; } // Обязательное

        [Required(ErrorMessage = "Возраст обязателен.")]
        public int? Age { get; set; } // Обязательное

        [Required(ErrorMessage = "Пол обязателен.")]
        public Gender? Sex { get; set; } // Обязательное

        [Required(ErrorMessage = "Основная цель обязательна.")]
        public string MainGoals { get; set; } // Обязательное

        [Required(ErrorMessage = "Уровень физической подготовки обязателен.")]
        public PhysicalFitnessLevel? LevelOfPhysicalFitness { get; set; } // Обязательное
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