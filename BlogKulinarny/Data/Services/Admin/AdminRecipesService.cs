using BlogKulinarny.Data.Helpers;
using BlogKulinarny.Models;
using BlogKulinarny.Models.AdminModels;
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

        public async Task<ChangesResult> addCategory(CategoryViewModel category)
        {
            try
            {
                var chekcer = _dbContext.categories.FirstOrDefault(u => u.name == category._newCategory);

                if (chekcer == null)
                {
                    Category newElement = new Category();
                    newElement.isAccepted = false;
                    newElement.name = category._newCategory;
                    _dbContext.categories.Add(newElement);
                    await _dbContext.SaveChangesAsync(); // Zapisz zmiany w bazie danych

                    return new ChangesResult(true, "Pomyslnie dodano kategorie");
                }
                else
                {
                    return new ChangesResult(false, "Juz wystepuje taka kategoria");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new ChangesResult(false, "wykryto wyjatek");
            }
        }

        public async Task<ChangesResult> lockCategory(CategoryViewModel category)
        {
            try
            {
                var checker = _dbContext.categories.FirstOrDefault(u => u.id == category._id);

                checker.isAccepted = false;

                _dbContext.categories.Update(checker);
                await _dbContext.SaveChangesAsync();
                return new ChangesResult(true, "Pomyslnie dodano kategorie");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new ChangesResult(false, "wykryto wyjatek");
            }
        }

        public async Task<ChangesResult> unlockCategory(CategoryViewModel category)
        {
            try
            {
                var checker = _dbContext.categories.FirstOrDefault(u => u.id == category._id);

                checker.isAccepted = true;

                _dbContext.categories.Update(checker);
                await _dbContext.SaveChangesAsync();
                return new ChangesResult(true, "Pomyslnie dodano kategorie");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new ChangesResult(false, "wykryto wyjatek");
            }
        }

        public async Task<ChangesResult> deleteCategory(CategoryViewModel category)
        {
            try
            {
                var checker = _dbContext.categories.FirstOrDefault(u => u.id == category._id);

                _dbContext.Remove(checker);

                await _dbContext.SaveChangesAsync();
                return new ChangesResult(true, "Pomyslnie dodano kategorie");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new ChangesResult(false, "wykryto wyjatek");
            }
        }
    }
}
