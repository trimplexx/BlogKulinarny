using System.ComponentModel.DataAnnotations;

namespace BlogKulinarny.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public int Rate { get; set; }

        [Display(Name = "RecipeId")]
        public int recipeId { get; set; }

        public int? isBlocked { get; set; }

        public Recipe recipe { get; set; }

        public int userId { get; set; }

        public User user { get; set; }
    }
}
