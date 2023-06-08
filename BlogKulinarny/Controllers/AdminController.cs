using BlogKulinarny.Data;
using BlogKulinarny.Data.Helpers;
using BlogKulinarny.Data.Services.Admin;
using BlogKulinarny.Models;
using BlogKulinarny.Models.AdminModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogKulinarny.Controllers;

/// <summary>
///     Kontroler dla funkcji administracyjnych.
/// </summary>
[TypeFilter(typeof(AuthorizeRankFilterFactory), Arguments = new object[] { 1 })]
public class AdminController : Controller
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AdminRecipesService _recipesService;
    private readonly AdminUsersService _usersService;

    /// <summary>
    ///     Tworzy nową instancję kontrolera AdminController.
    /// </summary>
    /// <param name="usersService">Serwis użytkowników.</param>
    /// <param name="adminRecipesService">Serwis przepisów.</param>
    /// <param name="dbContext">Kontekst bazy danych.</param>
    /// <param name="httpContextAccessor">Dostęp do kontekstu HTTP.</param>
    public AdminController(AdminUsersService usersService, AdminRecipesService adminRecipesService,
        AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _usersService = usersService;
        _dbContext = dbContext;
        _recipesService = adminRecipesService;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    ///     Wyświetla stronę główną panelu administracyjnego.
    /// </summary>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    ///     Wyświetla listę niepotwierdzonych użytkowników.
    /// </summary>
    public IActionResult UnconfirmedUsers()
    {
        try
        {
            var unacceptedUsers = _dbContext.users.Where(u => u.isAccepted == false).ToList();
            return View(unacceptedUsers);
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
    ///     Potwierdza użytkownika.
    /// </summary>
    /// <param name="model">Model użytkownika do potwierdzenia.</param>
    [HttpPost]
    public async Task<IActionResult> ConfirmUser(AdminUsersViewModel model)
    {
        await _usersService.ConfirmUser(model.userId);
        return Redirect("UnconfirmedUsers");
    }

    /// <summary>
    ///     Usuwa użytkownika.
    /// </summary>
    /// <param name="model">Model użytkownika do usunięcia.</param>
    public async Task<IActionResult> DeleteUser(AdminUsersViewModel model)
    {
        await _usersService.DeleteUser(model.userId);
        return Redirect("UnconfirmedUsers");
    }

    /// <summary>
    ///     Wyświetla listę niepotwierdzonych przepisów.
    /// </summary>
    public IActionResult UnconfirmedRecipes()
    {
        try
        {
            var recipes = _dbContext.recipes.Include(r => r.user).Include(r => r.recipesCategories)
                .ThenInclude(rc => rc.category).Where(r => r.isAccepted == false).ToList();
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
    ///     Potwierdza przepis.
    /// </summary>
    /// <param name="model">Model przepisu do potwierdzenia.</param>
    [HttpPost]
    public async Task<IActionResult> ConfirmRecipe(AdminUsersViewModel model)
    {
        await _recipesService.ConfirmRecipe(model.recipeId);
        return Redirect("UnconfirmedRecipes");
    }

    /// <summary>
    ///     Usuwa przepis.
    /// </summary>
    /// <param name="model">Model przepisu do usunięcia.</param>
    public async Task<IActionResult> DeleteRecipe(AdminUsersViewModel model)
    {
        await _recipesService.DeleteRecipe(model.recipeId);
        return Redirect("UnconfirmedRecipes");
    }

    /// <summary>
    ///     Wyświetla szczegóły użytkownika.
    /// </summary>
    public IActionResult DetailsUser()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Wyświetła listę przepisów.
    /// </summary>
    public IActionResult RecipeList()
    {
        try
        {
            var userId = _httpContextAccessor.HttpContext?.Session.GetString("UserId");

            // Spróbuj przekonwertować wartość userId na typ int
            if (!int.TryParse(userId, out _))
                return Unauthorized(); // Jeśli konwersja się nie powiedzie, zwróć false

            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var userRecipes = _dbContext.recipes.Include(r => r.user).Include(r => r.recipesCategories)
                .ThenInclude(rc => rc.category)
                .Where(r => r.isAccepted == true)
                .ToList();

            return View(userRecipes);
        }
        catch (Exception ex)
        {
            var errorModel = new ErrorViewModel
            {
                Message = ex.Message
            };
            return View("Error", errorModel);
        }
    }
}