using System.ComponentModel.DataAnnotations;

namespace BlogKulinarny.Models.UserModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Pole Hasło jest wymagane.")]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Pole Powtórz hasło jest wymagane.")]
        [DataType(DataType.Password)]
        [Display(Name = "Powtórz hasło")]
        [Compare("Password", ErrorMessage = "Hasła nie są zgodne.")]
        public string? ConfirmPassword { get; set; }
    }
}
