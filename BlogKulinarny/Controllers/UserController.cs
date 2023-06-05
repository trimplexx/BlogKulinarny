using BlogKulinarny.Data;
using BlogKulinarny.Data.Helpers;
using BlogKulinarny.Data.Services.Admin;
using BlogKulinarny.Data.Services.Users;
using BlogKulinarny.Models;
using BlogKulinarny.Models.RecipeModels;
using BlogKulinarny.Models.UserModels;
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
        private readonly IWebHostEnvironment _env;

        public UserController(UserRecipesService userRecipesService, AppDbContext dbContext, UserService userService, 
            IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _recipesService = userRecipesService;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _env = env;
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
        public async Task<IActionResult> EditUser()
        {
            var userId = _httpContextAccessor.HttpContext?.Session.GetString("UserId");
            int userIdAsInt;

            // Spróbuj przekonwertować wartość userId na typ int
            if (!int.TryParse(userId, out userIdAsInt))
            {
                return Unauthorized(); // Jeśli konwersja się nie powiedzie, zwróć false
            }
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

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
                    TempData["NotificationMessage"] = result ? "Adres email został zaktualizowany!" : "Wystąpił błąd podczas aktualizacji adresu email.";

                    if (result)
                    {
                        return RedirectToAction("EditUser", "User");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Wystąpił błąd podczas aktualizacji adresu email.";
                    }
                }
            }
            return RedirectToAction("EditUser", "User");
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

        [HttpPost]
        public async Task<IActionResult> UpdateAvatar([FromBody] EditUserModel model)
        {
            var userId = _httpContextAccessor.HttpContext?.Session.GetString("UserId");
            int userIdAsInt;

            // Spróbuj przekonwertować wartość userId na typ int
            if (!int.TryParse(userId, out userIdAsInt))
            {
                return Unauthorized(); // Jeśli konwersja się nie powiedzie, zwróć false
            }
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _dbContext.users.FirstOrDefaultAsync(u => u.Id == userIdAsInt);
            if (user == null)
            {
                return NotFound($"Nie znaleziono użytkownika z id: {userId}");
            }
            user.imageURL = model.AvatarUrl;
            _dbContext.users.Update(user);
            await _dbContext.SaveChangesAsync();
            _httpContextAccessor.HttpContext?.Session.SetString("Avatar", user.imageURL);
            return Ok();
        }


        public IActionResult AddRecipe()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecipe(AddRecipeViewModel recipe)
        {

            var userId = _httpContextAccessor.HttpContext?.Session.GetString("UserId");
            recipe.userId = Int32.Parse(userId);
            if (!string.IsNullOrEmpty(userId))
            {
                var result = await _recipesService.CreateRecipe(recipe);
            }

            return RedirectToAction("RecipeList", "User");
        }
    }
}

