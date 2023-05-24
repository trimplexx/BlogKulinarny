using BlogKulinarny.Data.Helpers;
using BlogKulinarny.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogKulinarny.Data.Services.Admin
{
    public class AdminRecipesService : IAdminRecipesService
    {
        private readonly AppDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AdminRecipesService(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ChangesResult> ConfirmRecipe(int recipeId)
        {
            try
            {
                Recipe recipe = await _dbContext.recipes.FirstOrDefaultAsync(r => r.id == recipeId);
                if (recipe != null)
                {
                    recipe.isAccepted = true;
                    _dbContext.recipes.Update(recipe);
                    await _dbContext.SaveChangesAsync(); // Zapisz zmiany w bazie danych
                    return new ChangesResult(true, "Pomyslnie zaakcetpowano przepis");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new ChangesResult(false, "wykryto wyjatek");
            }

            return new ChangesResult(false, "wystapil blad");
        }

        public async Task<ChangesResult> DeleteRecipe(int recipeId)
        {
            try
            {
                Recipe recipe = await _dbContext.recipes.FirstOrDefaultAsync(u => u.id == recipeId);
                if (recipe != null)
                {
                    _dbContext.recipes.Remove(recipe);
                    await _dbContext.SaveChangesAsync(); // Zapisz zmiany w bazie danych

                    // jeszcze trzeba tutaj dodac usuwanie wszystkich elementow przepisu

                    return new ChangesResult(true, "Pomyslnie usunieto przepis");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new ChangesResult(false, "wykryto wyjatek");
            }

            return new ChangesResult(false, "wystapil blad");
        }
    }
}
