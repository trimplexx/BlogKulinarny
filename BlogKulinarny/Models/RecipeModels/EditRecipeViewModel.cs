using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlogKulinarny.Models.RecipeModels
{
    public class EditRecipeViewModel
    {
        //recipe
        public int RecipeId { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "ImageUrl")]
        public string ImageURL { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Difficulty")]
        public int Difficulty { get; set; }

        [Display(Name = "AvgTime")]
        public int AvgTime { get; set; }

        [Display(Name = "Portions")]
        public int Portions { get; set; }

        //user
        [Display(Name = "User")]
        public int UserId { get; set; }

        //recipeElements
        public string SelectedTags { get; set; }

        public List<Category> Categories { get; set; }

        public string Ingredients { get; set; }

        public string SaveSteps { get; set; }

        public List<RecipeStep> Steps { get; set; }
    }

    public class RecipeStep
    {
        public string ImageURL { get; set; }
        public string Description { get; set; }
    }
}
