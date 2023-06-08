using System.Diagnostics;
using BlogKulinarny.Data;
using BlogKulinarny.Data.Services.Admin;
using BlogKulinarny.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogKulinarny.Controllers;

/// <summary>
///     Kontroler odpowiedzialny za obsługę stron głównych dla bloga kulinarnego.
/// </summary>
public class HomeController : Controller
{
    private readonly AppDbContext _dbContext;
    private readonly AdminUsersService _usersService;

    /// <summary>
    ///     Inicjalizuje nową instancję klasy <see cref="HomeController" /> z podanymi argumentami.
    /// </summary>
    /// <param name="logger">Logger dla kontrolera.</param>
    /// <param name="dbContext">Kontekst bazy danych bloga kulinarnego.</param>
    public HomeController(ILogger<HomeController> logger, AppDbContext dbContext, AdminUsersService usersService)
    {
        _dbContext = dbContext;
        _usersService = usersService;
    }

    /// <summary>
    ///     Wyświetla stronę główną bloga kulinarnego.
    /// </summary>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    ///     Wyświetla stronę z polityką prywatności.
    /// </summary>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    ///     Wyświetla stronę ze szczegółami przepisu.
    /// </summary>
    /// <param name="recipeId">Identyfikator przepisu do wyświetlenia.</param>
    public IActionResult RecipeDetails(int recipeId)
    {
        try
        {
            var recipe = _dbContext.recipes
                .Include(r => r.recipesCategories)
                .ThenInclude(rc => rc.category)
                .Include(r => r.recipeElements)
                .SingleOrDefault(r => r.isAccepted && r.id == recipeId);

            return View(recipe);
        }
        catch (Exception ex)
        {
            var errorViewModel = new ErrorViewModel
            {
                Message = ex.Message
            };

            return View("Error", errorViewModel);
        }
    }

    /// <summary>
    ///     Wyświetla listę przepisów z opcjami wyszukiwania i sortowania.
    /// </summary>
    /// <param name="searchForRecipe">Fraza do wyszukania przepisów.</param>
    /// <param name="sortOption1">Opcja sortowania po tytule.</param>
    /// <param name="sortOption2">Opcja sortowania po średnim czasie przygotowania.</param>
    /// <param name="sortOption3">Opcja sortowania po poziomie trudności.</param>
    /// <param name="unlock">Opcja odblokowania sortowania po poziomie trudności.</param>
    public IActionResult RecipesList(string searchForRecipe, string sortOption1, string sortOption2, string sortOption3,
        string unlock)
    {
        ViewBag.SearchForRecipe = searchForRecipe;
        try
        {
            var recipes = _dbContext.recipes
                .Include(r => r.recipesCategories)
                .ThenInclude(rc => rc.category)
                .Where(r => r.isAccepted == true)
                .OrderByDescending(r => r.id)
                .ToList();

            if (!string.IsNullOrWhiteSpace(searchForRecipe))
                recipes = _dbContext.recipes
                    .Include(r => r.recipesCategories)
                    .ThenInclude(rc => rc.category)
                    .Where(r => (r.isAccepted == true && r.title
                        .Contains(searchForRecipe)) || r.recipesCategories
                        .Any(rc => rc.category.name
                            .Contains(searchForRecipe)))
                    .OrderByDescending(r => r.id)
                    .ToList();

            if (sortOption1 == "true")
            {
                recipes = recipes.OrderBy(r => r.title).ToList();
                ViewBag.SortOption1 = sortOption1;
            }

            if (sortOption2 == "true")
            {
                recipes = recipes.OrderBy(r => r.avgTime).ToList();
                ViewBag.SortOption2 = sortOption2;
            }

            if (sortOption3 == "easiest" && unlock == "true")
            {
                recipes = recipes.OrderBy(r => r.difficulty).ToList();
                ViewBag.SortOption3 = sortOption3;
                ViewBag.unlock = unlock;
            }

            if (sortOption3 == "hardest" && unlock == "true")
            {
                recipes = recipes.OrderByDescending(r => r.difficulty).ToList();
                ViewBag.SortOption3 = sortOption3;
                ViewBag.unlock = unlock;
            }

            return View(recipes);
        }
        catch (Exception ex)
        {
            var errorViewModel = new ErrorViewModel
            {
                Message = ex.Message
            };

            return View("Error", errorViewModel);
        }
    }

    /// <summary>
    ///     Wyświetla stronę błędu.
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    /// <summary>
    ///     Wyświetla stronę z informacją o braku dostępu.
    /// </summary>
    public IActionResult NoAccess()
    {
        return View();
    }
}