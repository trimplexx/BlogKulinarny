using System.ComponentModel.DataAnnotations;

namespace BlogKulinarny.Models.RecipeModels
{
    public class AddRecipeViewModel
    {
        //recipe
        public string title { get; set; }
        public string imageURL { get; set; }
        public string description { get; set; }
        public int difficulty { get; set; }
        public int avgTime { get; set; }
        public int portions { get; set; }
        public int userId { get; set; }

        //recipeElements
        //List<RecipeElements> RecipeElements { get; set; }
    }
}
