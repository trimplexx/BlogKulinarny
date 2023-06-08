using System.ComponentModel.DataAnnotations;

namespace BlogKulinarny.Models.AuthModels;

public class ResetPasswordViewModel
{
    [Required(ErrorMessage = "Pole Adres Email jest wymagane.")]
    [EmailAddress(ErrorMessage = "Wprowadź poprawny adres email.")]
    public string Email { get; set; }
}