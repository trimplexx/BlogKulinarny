using BlogKulinarny.Data.Helpers;
using BlogKulinarny.Models;

namespace BlogKulinarny.Data.Services.Users
{
    public interface IUserRecipesService
    {
        Task<ChangesResult> CreateRecipe(Recipe recipe);

        Task<ChangesResult> UpdateRecipe(Recipe recipe);

        Task<ChangesResult> DeleteRecipe(int recipeId);
    }
}
