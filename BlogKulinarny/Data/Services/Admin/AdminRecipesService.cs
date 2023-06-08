using BlogKulinarny.Data.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BlogKulinarny.Data.Services.Admin;

/// <summary>
///     Serwis zarządzania przepisami dla panelu administratora w projekcie bloga kulinarnego w ASP.NET MVC Core.
/// </summary>
public class AdminRecipesService : IAdminRecipesService
{
    private readonly AppDbContext _dbContext;

    /// <summary>
    ///     Inicjalizuje nową instancję klasy AdminRecipesService z określonym kontekstem bazy danych i dostępem do kontekstu
    ///     HTTP.
    /// </summary>
    /// <param name="dbContext">Kontekst bazy danych aplikacji.</param>
    public AdminRecipesService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    ///     Potwierdza przepis o podanym identyfikatorze.
    /// </summary>
    /// <param name="recipeId">Identyfikator przepisu do potwierdzenia.</param>
    /// <returns>Obiekt ChangesResult zawierający informacje o wyniku operacji.</returns>
    public async Task<ChangesResult> ConfirmRecipe(int recipeId)
    {
        try
        {
            var recipe = await _dbContext.recipes.FirstOrDefaultAsync(r => r.id == recipeId);
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

    /// <summary>
    ///     Usuwa przepis o podanym identyfikatorze.
    /// </summary>
    /// <param name="recipeId">Identyfikator przepisu do usunięcia.</param>
    /// <returns>Obiekt ChangesResult zawierający informacje o wyniku operacji.</returns>
    public async Task<ChangesResult> DeleteRecipe(int recipeId)
    {
        try
        {
            var recipe = await _dbContext.recipes.FirstOrDefaultAsync(u => u.id == recipeId);
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