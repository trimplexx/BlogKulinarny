
using BlogKulinarny.Data;
using BlogKulinarny.Data.Helpers;
using BlogKulinarny.Data.Services.Admin;
using BlogKulinarny.Data.Services.Users;
using BlogKulinarny.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Exchange.WebServices.Data;

namespace BlogKulinarny.Controllers
{
    [TypeFilter(typeof(AuthorizeRankFilterFactory), Arguments = new object[] { 0 })]
    public class UserController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly UserRecipesService _recipesService;
        private readonly UserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(UserRecipesService userRecipesService, AppDbContext dbContext, UserService userService, 
            IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _recipesService = userRecipesService;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult LoadPartialView(string viewName)
        {
            return PartialView(viewName);
        }
        
        public IActionResult RecipeList()
        {
            try
            {
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
                    //Message = "An error occurred while retrieving recipes.",
                    //Exception = ex
                };
                return View("Error", errorModel);
            }
        }

        [HttpGet]
        public IActionResult EditUser()
        {
            // Pobierz dane użytkownika (możesz użyć np. Identity lub serwisu użytkowników)
            var user = new User(); // Pobierz użytkownika do edycji

            // Utwórz model edycji użytkownika na podstawie danych z bazy
            var editUserModel = new EditUserModel
            {
                Login = user.login,
                Email = user.mail
            };

            return View(editUserModel);
        }

        [HttpPost]
        public IActionResult EditUser(EditUserModel model)
        {
            if (ModelState.IsValid)
            {
                // Zapisz zmiany w bazie danych (np. poprzez serwis użytkowników lub Identity)
                // Przetwarzaj model i zaktualizuj dane użytkownika

                // Przykładowe zapisanie zmian:
                var user = new User(); // Pobierz użytkownika do edycji
                user.login = model.Login;
                user.mail = model.Email;

                // Zapisz użytkownika w bazie danych

                return RedirectToAction("Index", "Home"); // Przekieruj na inny widok po zapisaniu zmian
            }

            // Jeśli walidacja modelu nie powiedzie się, zwróć widok edycji wraz z błędami
            return View(model);
        }
        

        [HttpPost]
        public async Task<IActionResult> DeleteAccount(DeleteAccountModel model)
        {
            if (ModelState.IsValid)
            {
                
                var userId = _httpContextAccessor.HttpContext?.Session.GetString("UserId");
                var success = userId != null && await _userService.DeleteAccountAsync(userId, model.Password);

                if (success)
                {
                    // Usunięcie użytkownika z sesji i przekierowanie do strony logowania
                    _httpContextAccessor.HttpContext.Session.Clear();
                    _httpContextAccessor.HttpContext.Session.Remove("UserId");
                    _httpContextAccessor.HttpContext.Session.Remove("Login");
                    _httpContextAccessor.HttpContext.Session.Remove("Email");
                    TempData["NotificationMessageType"] = "success";
                    TempData["NotificationMessage"] = "Poprawnie usunięto konto.";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["NotificationMessageType"] = "error";
                    TempData["NotificationMessage"] = "Konto nie zostało usunięte, podałeś błędne hasło!";
                }
            }
            return View("EditUser");
        }

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
                    {
                        return RedirectToAction("EditUser", "User");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = result.ErrorMessage;
                    }
                }
            }
            return RedirectToAction("EditUser", "User");
        }
    }
}

