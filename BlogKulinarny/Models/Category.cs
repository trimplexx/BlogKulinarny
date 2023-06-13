using System.ComponentModel.DataAnnotations;

namespace BlogKulinarny.Models
{
    public class Category
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Name")]
        public string name { get; set; }

        [Display(Name = "isAccepted")]
        public bool isAccepted { get; set; }

        //Relationships
        public List<RecipesCategory> recipesCategories { get; set; }
    }
}
