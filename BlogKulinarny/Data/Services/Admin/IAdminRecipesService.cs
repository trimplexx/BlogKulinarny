using BlogKulinarny.Data.Helpers;

namespace BlogKulinarny.Data.Services.Admin
{
    public interface IAdminRecipesService
    {
        public Task<ChangesResult> ConfirmRecipe(int recipeId);

        public Task<ChangesResult> DeleteRecipe(int recipeId);
    }
}
