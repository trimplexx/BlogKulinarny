using BlogKulinarny.Data;
using BlogKulinarny.Data.Services.Admin;
using BlogKulinarny.Data.Services.Users;
using BlogKulinarny.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogKulinarny.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly UserRecipesService _recipesService;

        public UserController(UserRecipesService userRecipesService, AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _recipesService = userRecipesService;
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
    }
}
