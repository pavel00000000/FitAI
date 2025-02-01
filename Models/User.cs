using System.ComponentModel.DataAnnotations;

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
