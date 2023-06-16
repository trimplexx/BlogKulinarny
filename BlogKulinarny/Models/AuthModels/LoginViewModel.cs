using System.ComponentModel.DataAnnotations;

namespace BlogKulinarny.Models.AuthModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Pole Adres Email jest wymagane.")]
    [EmailAddress(ErrorMessage = "Wprowadź poprawny adres email.")]
    public string EmailOrLogin { get; set; }

    [Required(ErrorMessage = "Pole Hasło jest wymagane.")]
    public string? Password { get; set; }
}