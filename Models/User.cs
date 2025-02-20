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

       
    }

    
}

//public class UserProfile
//{
//    [Key, ForeignKey("User")]
//    public int UserID { get; set; }

//    public User User { get; set; } // Навигационное свойство

//    public string FullName { get; set; } 
//    public string BodyType { get; set; } 
//    public int? Height { get; set; } 
//    public int? Weight { get; set; } 
//    public int? Age { get; set; } 
//    public Gender? Sex { get; set; } 
//    public string MainGoals { get; set; }
//    public PhysicalFitnessLevel? LevelOfPhysicalFitness { get; set; } 
//}

//public enum Gender
//{
//    Male,
//    Female
//}

//public enum PhysicalFitnessLevel
//{
//    Beginner,
//    Intermediate,
//    Advanced
//}