using System.ComponentModel.DataAnnotations;

namespace BlogKulinarny.Models
{
    public class EditUserModel
    {
        [Required(ErrorMessage = "Pole Login jest wymagane.")]
        [Display(Name = "Login")]
        public string? Login { get; set; }
        
        [EmailAddress(ErrorMessage = "Nieprawidłowy format adresu email.")]
        [Display(Name = "Adres email")]
        public string? Email { get; set; }
        
        [Required(ErrorMessage = "Wymagane podanie starego hasła.")]
        [DataType(DataType.Password)]
        [Display(Name = "Stare hasło")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Wymagane podanie nowego hasła.")]
        [DataType(DataType.Password)]
        [Display(Name = "Nowe hasło")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Wymagane potwierdzenie nowego hasła.")]
        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź nowe hasło")]
        [Compare("NewPassword", ErrorMessage = "Nowe hasło i potwierdzenie nowego hasła muszą się zgadzać.")]
        public string ConfirmNewPassword { get; set; }
    }
}
