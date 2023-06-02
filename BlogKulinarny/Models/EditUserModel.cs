using System.ComponentModel.DataAnnotations;

namespace BlogKulinarny.Models
{
    public class EditUserModel
    {
        public string? AvatarUrl { get; set; }
        
        [Required(ErrorMessage = "Pole Login jest wymagane.")]
        [Display(Name = "Login")]
        public string? Login { get; set; }
        
        [EmailAddress(ErrorMessage = "Nieprawidłowy format adresu email.")]
        [Display(Name = "Adres email")]
        public string? Email { get; set; }
    }
}
