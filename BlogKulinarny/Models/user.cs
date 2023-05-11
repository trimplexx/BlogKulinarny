using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace BlogKulinarny.Models
{
    public class user
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Login")]
        public string Login { get; set; }

        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Mail")]
        public string Mail { get; set; }

        [Display(Name = "ActivationStatus")]
        public bool IsAccepted { get; set; }

        [Display(Name = "Role")]
        public int RoleId { get; set; }

        //Relationships
    }
}
