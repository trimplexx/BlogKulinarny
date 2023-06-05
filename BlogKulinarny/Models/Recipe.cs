using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace BlogKulinarny.Models
{
    public class Recipe // brakuje relacji z category posts
    {
        public Recipe()
        {
            recipesCategories = new List<RecipesCategory>();
        }

        public Recipe(bool isAccepted, string title, string imageURL, string description, int difficulty, int avgTime, int portions, int userId)
        {
            this.isAccepted = isAccepted;
            this.title = title;
            this.imageURL = imageURL;
            this.description = description;
            this.difficulty = difficulty;
            this.avgTime = avgTime;
            this.portions = portions;
            this.userId = userId;
        }

        [Key]
        public int id { get; set; }

        [Display(Name = "ActivationStatus")]
        public bool isAccepted { get; set; }

        [Display(Name = "Title")]
        public string title { get; set; }

        [Display(Name = "ImageUrl")]
        public string imageURL { get; set; }

        [Display(Name = "Description")]
        public string description { get; set; }

        [Display(Name = "Difficulty")]
        public int difficulty { get; set; }

        [Display(Name = "AvgTime")]
        public int avgTime { get; set; }

        [Display(Name = "Portions")]
        public int portions { get; set; }

        //user
        [Display(Name = "User")]
        public int userId { get; set; }
        public User user { get; set; }

        //Relationships
        public List<RecipesCategory> recipesCategories { get; set; }
    }
}
