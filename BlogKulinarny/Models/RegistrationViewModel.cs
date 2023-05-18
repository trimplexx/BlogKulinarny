using System.ComponentModel.DataAnnotations;
namespace BlogKulinarny.Models;

public class RegistrationViewModel
{
    [Required(ErrorMessage = "Pole Login jest wymagane.")]
    [Display(Name = "Login")]
    public string Login { get; set; }

    [Required(ErrorMessage = "Pole Hasło jest wymagane.")]
    [DataType(DataType.Password)]
    [Display(Name = "Hasło")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Pole Powtórz hasło jest wymagane.")]
    [DataType(DataType.Password)]
    [Display(Name = "Powtórz hasło")]
    [Compare("Password", ErrorMessage = "Hasła nie są zgodne.")]
    public string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "Pole Adres email jest wymagane.")]
    [EmailAddress(ErrorMessage = "Nieprawidłowy format adresu email.")]
    [Display(Name = "Adres email")]
    public string Email { get; set; }
}

