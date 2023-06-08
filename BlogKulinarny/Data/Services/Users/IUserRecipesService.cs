using BlogKulinarny.Data.Helpers;
using BlogKulinarny.Models;
using BlogKulinarny.Models.RecipeModels;

namespace BlogKulinarny.Data.Services.Users;

public interface IUserRecipesService
{
    Task<ChangesResult> CreateRecipe(AddRecipeViewModel recipe);

    Task<ChangesResult> UpdateRecipe(Recipe recipe);

    Task<ChangesResult> DeleteRecipe(int recipeId);
}