using BlogKulinarny.Data.Helpers;
using BlogKulinarny.Models;
using BlogKulinarny.Models.RecipeModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Xml.Linq;

namespace BlogKulinarny.Data.Services.Users
{
    public class UserRecipesService : IUserRecipesService
    {
        private readonly AppDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserRecipesService(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ChangesResult> CreateRecipe(AddRecipeViewModel recipe)
        {
            try
            {
                //dodanie przepisu do bazy
                var recp = new Recipe(false, recipe.title,recipe.imageURL,recipe.description, 
                    ConvertRange(recipe.difficulty), recipe.avgTime, recipe.portions,recipe.userId); 
                _dbContext.recipes.Add(recp);

                //dodawanie tagów do bazy
                string[] tagArray = recipe.selectedTags?.Split(',');
                foreach (var category in tagArray)
                {

                    Category cat = _dbContext.categories.FirstOrDefault(c => c.name == category.ToString());
                    RecipesCategory recCat = new RecipesCategory();

                    recCat.category = cat;
                    recCat.categoryId = cat.id;
                    recCat.recipeId = recp.id;
                    recCat.recipe = recp;
                    _dbContext.recipesCategories.Add(recCat);
                }

                int NoOfList = 0;
                //dodawanie listy składników do bazy
                RecipeElements eleIng = new RecipeElements();

                eleIng.noOfList = NoOfList;
                eleIng.description = recipe.ingredients;
                eleIng.recipeId = recp.id;
                eleIng.recipe = recp;
                eleIng.imageURL = "brak";
                NoOfList++;
                _dbContext.recipesElements.Add(eleIng);

                string[] Array = recipe.saveSteps?.Split(',');
                for (int i = 0; i < Array.Length; i += 2)
                {
                    RecipeElements ele = new RecipeElements();

                    ele.noOfList = NoOfList;
                    ele.description = Array[i] ?? "brak opisu";
                    ele.recipeId = recp.id;
                    ele.recipe = recp;
                    ele.imageURL = Array[i+1];
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

        public async Task<ChangesResult> ChangeRecipeValues(EditRecipeViewModel recipe)
        {
            // Edytowany przepis
            var editedRecipe = _dbContext.recipes
                .Include(r => r.recipeElements)
                .SingleOrDefault(r => r.isAccepted && r.id == recipe.editRecipe.Id);

            if (editedRecipe != null)
            {
                // Aktualizacja właściwości przepisu
                editedRecipe.title = recipe.editRecipe.title;
                editedRecipe.imageURL = recipe.editRecipe.imageURL;
                editedRecipe.description = recipe.editRecipe.description;
                editedRecipe.difficulty = ConvertRange(recipe.editRecipe.difficulty);
                editedRecipe.avgTime = recipe.editRecipe.avgTime;
                editedRecipe.portions = recipe.editRecipe.portions;

                // Usunięcie istniejących elementów przepisu
                _dbContext.recipesElements.RemoveRange(editedRecipe.recipeElements);

                // Dodanie nowych elementów przepisu
                int noOfList = 0;
                var newElements = new List<RecipeElements>();

                // Dodawanie listy składników do bazy
                var ingredientElement = new RecipeElements()
                {
                    noOfList = noOfList,
                    description = recipe.editRecipe.ingredients,
                    imageURL = "brak"
                };
                newElements.Add(ingredientElement);
                noOfList++;

                // Tworzenie listy kroków
                string[] stepsArray = recipe.editRecipe.saveSteps?.Split(',');
                for (int i = 0; i < stepsArray.Length; i += 2)
                {
                    var stepElement = new RecipeElements()
                    {
                        noOfList = noOfList,
                        description = stepsArray[i] ?? "brak opisu",
                        imageURL = stepsArray[i + 1]
                    };
                    newElements.Add(stepElement);
                    noOfList++;
                }

                editedRecipe.recipeElements = newElements;

                _dbContext.Update(editedRecipe);
                await _dbContext.SaveChangesAsync(); // Zapisz zmiany w bazie danych
                return new ChangesResult(true, "Pomyślnie edytowano przepis");
            }
            else
            {
                return new ChangesResult(false, "Przepis nie został odnaleziony.");
            }

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

        //metody pomocnicze

        public int ConvertRange(int range)
        {
            if (range <= 25)
            {
                range = 1;
            }
            else if (range <= 60)
            {
                range = 2;
            }
            else if (range <= 80)
            {
                range = 3;
            }
            else
            {
                range = 4;
            }
            return 1;
        }
    }
}
