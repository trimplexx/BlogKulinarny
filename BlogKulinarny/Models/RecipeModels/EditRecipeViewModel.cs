 using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlogKulinarny.Models.RecipeModels
{
    public class EditRecipeViewModel
    {
        //recipe
        public Recipe recipe { get; set; }

        public AddRecipeViewModel editRecipe { get; set; }
    }
}
