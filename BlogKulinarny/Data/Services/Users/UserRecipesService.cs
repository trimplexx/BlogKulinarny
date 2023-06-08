using BlogKulinarny.Data.Helpers;
using BlogKulinarny.Models;
using BlogKulinarny.Models.RecipeModels;
using Microsoft.EntityFrameworkCore;

namespace BlogKulinarny.Data.Services.Users;

/// <summary>
///     Serwis zarządzający przepisami użytkownika.
/// </summary>
public class UserRecipesService : IUserRecipesService
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    ///     Inicjalizuje nową instancję klasy <see cref="UserRecipesService" />.
    /// </summary>
    /// <param name="dbContext">Kontekst bazy danych.</param>
    /// <param name="httpContextAccessor">Dostęp do kontekstu HTTP.</param>
    public UserRecipesService(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    ///     Tworzy nowy przepis.
    /// </summary>
    /// <param name="recipe">Model przepisu do dodania.</param>
    /// <returns>Wynik operacji.</returns>
    public async Task<ChangesResult> CreateRecipe(AddRecipeViewModel recipe)
    {
        try
        {
            // Dodanie przepisu do bazy danych
            var recp = new Recipe(false, recipe.title, recipe.imageURL, recipe.description,
                ConvertRange(recipe.difficulty), recipe.avgTime, recipe.portions, recipe.userId);
            _dbContext.recipes.Add(recp);

            // Dodawanie tagów do bazy danych
            var tagArray = recipe.selectedTags?.Split(',');
            foreach (var category in tagArray)
            {
                var cat = _dbContext.categories.FirstOrDefault(c => c.name == category);
                var recCat = new RecipesCategory();

                recCat.category = cat;
                recCat.categoryId = cat.id;
                recCat.recipeId = recp.id;
                recCat.recipe = recp;
                _dbContext.recipesCategories.Add(recCat);
            }

            var NoOfList = 0;
            // Dodawanie listy składników do bazy danych
            var eleIng = new RecipeElements();

            eleIng.noOfList = NoOfList;
            eleIng.description = recipe.ingredients;
            eleIng.recipeId = recp.id;
            eleIng.recipe = recp;
            eleIng.imageURL = "brak";
            NoOfList++;
            _dbContext.recipesElements.Add(eleIng);

            // Dodawanie kroków przepisu do bazy danych
            var Array = recipe.saveSteps?.Split(',');
            for (var i = 0; i < Array.Length; i += 2)
            {
                var ele = new RecipeElements();

                ele.noOfList = NoOfList;
                ele.description = Array[i] ?? "brak opisu";
                ele.recipeId = recp.id;
                ele.recipe = recp;
                ele.imageURL = Array[i + 1];
                _dbContext.recipesElements.Add(ele);
                NoOfList++;
            }

            await _dbContext.SaveChangesAsync();
            return new ChangesResult(true, "Pomyslnie dodano przepis");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new ChangesResult(false, "wykryto wyjatek");
        }
    }

    /// <summary>
    ///     Aktualizuje przepis.
    /// </summary>
    /// <param name="recipe">Przepis do zaktualizowania.</param>
    /// <returns>Wynik operacji.</returns>
    public async Task<ChangesResult> UpdateRecipe(Recipe recipe)
    {
        try
        {
            _dbContext.recipes.Update(recipe);
            await _dbContext.SaveChangesAsync();
            return new ChangesResult(true, "Pomyslnie edytowano przepis");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new ChangesResult(false, "wykryto wyjatek");
        }
    }

    /// <summary>
    ///     Usuwa przepis.
    /// </summary>
    /// <param name="recipeId">Identyfikator przepisu do usunięcia.</param>
    /// <returns>Wynik operacji.</returns>
    public async Task<ChangesResult> DeleteRecipe(int recipeId)
    {
        try
        {
            var recipe = await _dbContext.recipes.FirstOrDefaultAsync(u => u.id == recipeId);
            if (recipe != null)
            {
                _dbContext.recipes.Remove(recipe);
                await _dbContext.SaveChangesAsync();

                // Tutaj trzeba dodać usuwanie wszystkich elementów przepisu

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

    /// <summary>
    ///     Konwertuje wartość zakresu trudności.
    /// </summary>
    /// <param name="range">Wartość zakresu trudności do konwersji.</param>
    /// <returns>Skonwertowana wartość zakresu.</returns>
    public int ConvertRange(int range)
    {
        if (range <= 25)
            range = 1;
        else if (range <= 60)
            range = 2;
        else if (range <= 80)
            range = 3;
        else
            range = 4;
        return 1;
    }
}