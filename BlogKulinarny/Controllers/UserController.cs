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
        public async Task<IActionResult> ChangePassword(EditUserModel model)
        {
            if (ModelState.IsValid)
            {
                var login = _httpContextAccessor.HttpContext.Session.GetString("login");
                if (login != null)
                {
                    var result = await _userService.ChangePasswordAsync(login, model.OldPassword, model.NewPassword, model.ConfirmNewPassword);
                    if (result.Success)
                    {
                        TempData["NotificationMessageType"] = "success";
                        TempData["NotificationMessage"] = "Poprawnie zmieniono hasło!";
                        return RedirectToAction("EditUser", "User");
                    }
                    else
                    {
                        TempData["NotificationMessageType"] = "error";
                        TempData["NotificationMessage"] = "Błąd zmiany hasła!";
                        return RedirectToAction("EditUser", "User");
                    }
                }
            }
            return View("EditUser", model);
        }
    }
}
