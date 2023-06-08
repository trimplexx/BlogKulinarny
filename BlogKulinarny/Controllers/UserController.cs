using BlogKulinarny.Data;
using BlogKulinarny.Data.Helpers;
using BlogKulinarny.Data.Services.Users;
using BlogKulinarny.Models;
using BlogKulinarny.Models.RecipeModels;
using BlogKulinarny.Models.UserModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogKulinarny.Controllers;

/// <summary>
///     Kontroler obsługujący akcje związane z użytkownikiem.
/// </summary>
[TypeFilter(typeof(AuthorizeRankFilterFactory), Arguments = new object[] { 0 })]
public class UserController : Controller
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserRecipesService _recipesService;
    private readonly UserService _userService;

    public UserController(UserRecipesService userRecipesService, AppDbContext dbContext, UserService userService,
        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _recipesService = userRecipesService;
        _userService = userService;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    ///     Akcja odpowiedzialna za wyświetlenie strony głównej użytkownika.
    /// </summary>
    /// <returns>Widok strony głównej użytkownika.</returns>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    ///     Akcja odpowiedzialna za ładowanie widoku częściowego.
    /// </summary>
    /// <param name="viewName">Nazwa widoku częściowego.</param>
    /// <returns>Widok częściowy.</returns>
    [HttpGet]
    public IActionResult LoadPartialView(string viewName)
    {
        return PartialView(viewName);
    }

    /// <summary>
    ///     Akcja odpowiedzialna za wyświetlenie listy przepisów użytkownika.
    /// </summary>
    /// <returns>Widok listy przepisów użytkownika.</returns>
    public IActionResult RecipeList()
    {
        try
        {
            var userId = _httpContextAccessor.HttpContext?.Session.GetString("UserId");
            int userIdAsInt;

            if (!int.TryParse(userId, out userIdAsInt)) return Unauthorized();

            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var userRecipes = _dbContext.recipes.Include(r => r.user).Include(r => r.recipesCategories)
                .ThenInclude(rc => rc.category)
                .Where(r => r.isAccepted == true).Where(r => r.userId == userIdAsInt)
                .ToList();

            return View(userRecipes);
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
    ///     Akcja odpowiedzialna za wyświetlenie formularza edycji użytkownika.
    /// </summary>
    /// <returns>Widok formularza edycji użytkownika.</returns>
    [HttpGet]
    public async Task<IActionResult> EditUser()
    {
        var userId = _httpContextAccessor.HttpContext?.Session.GetString("UserId");
        int userIdAsInt;

        // Spróbuj przekonwertować wartość userId na typ int
        if (!int.TryParse(userId, out userIdAsInt))
            return Unauthorized(); // Jeśli konwersja się nie powiedzie, zwróć false

        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var user = await _dbContext.users.FirstOrDefaultAsync(u => u.Id == userIdAsInt);

        // Utwórz model edycji użytkownika na podstawie danych z bazy
        var editUserModel = new EditUserModel
        {
            Login = user?.login,
            Email = user?.mail,
            AvatarUrl = user?.imageURL
        };

        return View(editUserModel);
    }

    /// <summary>
    ///     Akcja odpowiedzialna za zapisanie zmian w edytowanym użytkowniku.
    /// </summary>
    /// <param name="model">Model edycji użytkownika.</param>
    /// <returns>Przekierowanie do akcji edycji użytkownika.</returns>
    [HttpPost]
    public async Task<IActionResult> EditUser(EditUserModel model)
    {
        if (ModelState.IsValid)
        {
            var userId = _httpContextAccessor.HttpContext?.Session.GetString("UserId");
            if (!string.IsNullOrEmpty(userId))
            {
                var result = await _userService.UpdateUserEmailAsync(userId, model.Email);

                TempData["NotificationMessageType"] = result ? "success" : "error";
                TempData["NotificationMessage"] = result
                    ? "Adres email został zaktualizowany!"
                    : "Wystąpił błąd podczas aktualizacji adresu email.";

                if (result)
                    return RedirectToAction("EditUser", "User");
                ViewBag.ErrorMessage = "Wystąpił błąd podczas aktualizacji adresu email.";
            }
        }

        return RedirectToAction("EditUser", "User");
    }

    /// <summary>
    ///     Akcja odpowiedzialna za usunięcie konta użytkownika.
    /// </summary>
    /// <param name="model">Model danych do usunięcia konta.</param>
    /// <returns>Widok edycji użytkownika lub przekierowanie do strony głównej.</returns>
    [HttpPost]
    public async Task<IActionResult> DeleteAccount(DeleteAccountModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var userId = _httpContextAccessor.HttpContext?.Session.GetString("UserId");
                var success = userId != null && await _userService.DeleteAccountAsync(userId, model.Password);

                if (success)
                {
                    if (_httpContextAccessor.HttpContext != null)
                    {
                        _httpContextAccessor.HttpContext.Session.Clear();
                        _httpContextAccessor.HttpContext.Session.Remove("UserId");
                        _httpContextAccessor.HttpContext.Session.Remove("Login");
                        _httpContextAccessor.HttpContext.Session.Remove("Email");
                    }

                    TempData["NotificationMessageType"] = "success";
                    TempData["NotificationMessage"] = "Poprawnie usunięto konto.";
                    return RedirectToAction("Index", "Home");
                }

                TempData["NotificationMessageType"] = "error";
                TempData["NotificationMessage"] = "Konto nie zostało usunięte, podałeś błędne hasło!";
            }
        }
        catch (Exception ex)
        {
            var errorViewModel = new ErrorViewModel
            {
                Message = ex.Message
            };

            return View("Error", errorViewModel);
        }

        return View("EditUser");
    }

    /// <summary>
    ///     Akcja odpowiedzialna za zmianę hasła użytkownika.
    /// </summary>
    /// <param name="model">Model danych do zmiany hasła.</param>
    /// <returns>Przekierowanie do akcji edycji użytkownika.</returns>
    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
    {
        if (ModelState.IsValid)
        {
            var userId = _httpContextAccessor.HttpContext?.Session.GetString("UserId");
            if (!string.IsNullOrEmpty(userId))
            {
                var result = await _userService.ChangePasswordAsync(userId, model.OldPassword, model.NewPassword);
                TempData["NotificationMessageType"] = result.Success ? "success" : "error";
                TempData["NotificationMessage"] = result.ErrorMessage;

                if (result.Success)
                    return RedirectToAction("EditUser", "User");
                ViewBag.ErrorMessage = result.ErrorMessage;
            }
        }

        return RedirectToAction("EditUser", "User");
    }

    /// <summary>
    ///     Akcja odpowiedzialna za aktualizację awatara użytkownika.
    /// </summary>
    /// <param name="model">Model danych do aktualizacji awatara.</param>
    /// <returns>Widok edycji użytkownika lub brak dostępu.</returns>
    [HttpPost]
    [HttpPost]
    public async Task<IActionResult> UpdateAvatar([FromBody] EditUserModel model)
    {
        try
        {
            var userId = _httpContextAccessor.HttpContext?.Session.GetString("UserId");
            int userIdAsInt;

            if (!int.TryParse(userId, out userIdAsInt)) return Unauthorized();

            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var user = await _dbContext.users.FirstOrDefaultAsync(u => u.Id == userIdAsInt);
            if (user == null) return NotFound($"Nie znaleziono użytkownika z id: {userId}");
            user.imageURL = model.AvatarUrl;
            _dbContext.users.Update(user);
            await _dbContext.SaveChangesAsync();
            if (user.imageURL != null) _httpContextAccessor.HttpContext?.Session.SetString("Avatar", user.imageURL);
            return Ok();
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
    ///     Akcja odpowiedzialna za wyświetlanie formularza dodawania przepisu.
    /// </summary>
    /// <returns>Widok formularza dodawania przepisu.</returns>
    public IActionResult AddRecipe()
    {
        var model = new AddRecipeViewModel();
        var categories = _dbContext.categories.ToList();
        model.categories = categories;
        return View(model);
    }

    /// <summary>
    ///     Akcja odpowiedzialna za tworzenie nowego przepisu.
    /// </summary>
    /// <param name="recipe">Model danych nowego przepisu.</param>
    /// <returns>Przekierowanie do listy przepisów użytkownika.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateRecipe(AddRecipeViewModel recipe)
    {
        try
        {
            var userId = _httpContextAccessor.HttpContext?.Session.GetString("UserId");
            if (userId != null)
            {
                recipe.userId = int.Parse(userId);
                if (!string.IsNullOrEmpty(userId))
                {
                    await _recipesService.CreateRecipe(recipe);
                }
            }

            return RedirectToAction("RecipeList", "User");
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
}