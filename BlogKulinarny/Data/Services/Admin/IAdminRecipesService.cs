using BlogKulinarny.Data.Helpers;
using BlogKulinarny.Models.AdminModels;

namespace BlogKulinarny.Data.Services.Admin
{
    public interface IAdminRecipesService
    {
        public Task<ChangesResult> ConfirmRecipe(int recipeId);

        public Task<ChangesResult> DeleteRecipe(int recipeId);

        public Task<ChangesResult> addCategory(CategoryViewModel category);

        public Task<ChangesResult> lockCategory(CategoryViewModel category);

        public Task<ChangesResult> unlockCategory(CategoryViewModel category);

        public Task<ChangesResult> deleteCategory(CategoryViewModel category);
    }
}
