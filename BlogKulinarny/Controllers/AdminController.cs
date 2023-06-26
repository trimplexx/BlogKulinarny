using BlogKulinarny.Data;
using BlogKulinarny.Data.Helpers;
using BlogKulinarny.Data.Services.Admin;
using BlogKulinarny.Models;
using BlogKulinarny.Models.AdminModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogKulinarny.Controllers
{
    [TypeFilter(typeof(AuthorizeRankFilterFactory), Arguments = new object[] { 1 })]
    public class AdminController : Controller
    {
        private readonly AdminUsersService _usersService;
        private readonly AdminRecipesService _recipesService;
        private readonly AppDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminController(AdminUsersService UsersService,AdminRecipesService adminRecipesService, AppDbContext dbContext, IHttpContextAccessor httpContextAccessor) {
            _usersService = UsersService;
            _dbContext = dbContext;
            _recipesService = adminRecipesService;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }

        // uzytkownicy
        public IActionResult UnconfirmedUsers()
        {
            try
            {
                var unacceptedUsers = _dbContext.users.Where(u => u.isAccepted==false).ToList();
                return View(unacceptedUsers);
            }
            catch (Exception ex)
            {
                var errorViewModel = new ErrorViewModel
                {
                    Message = ex.Message,
                };

                return View("Error", errorViewModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmUser(AdminUsersViewModel model)
        {
            await _usersService.ConfirmUser(model.userId);
            return Redirect("UnconfirmedUsers");
        }

        public async Task<IActionResult> DeleteUser(AdminUsersViewModel model)
        {
            await _usersService.DeleteUser(model.userId);
            return Redirect("UnconfirmedUsers");
        }

        public IActionResult DetailsUser()
        {
            throw new NotImplementedException();
        }

        //przepis
        public IActionResult UnconfirmedRecipes()
        {
            try
            {
                var recipes = _dbContext.recipes.Include(r => r.user).Include(r => r.recipesCategories).
                    ThenInclude(rc => rc.category).Where(r => r.isAccepted == false).ToList();
                return View(recipes);
            }
            catch (Exception ex)
            {
                var errorViewModel = new ErrorViewModel
                {
                    Message = ex.Message,
                };

                return View("Error", errorViewModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmRecipe(AdminUsersViewModel model)
        {
            await _recipesService.ConfirmRecipe(model.recipeId);
            return Redirect("UnconfirmedRecipes");
        }

        public async Task<IActionResult> DeleteRecipe(AdminUsersViewModel model)
        {
            await _recipesService.DeleteRecipe(model.recipeId);
            return Redirect("RecipeList");
        }

        public IActionResult RecipeList()
        {
            try
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

                var userRecipes = _dbContext.recipes.Include(r => r.user).Include(r => r.recipesCategories)
                    .ThenInclude(rc => rc.category).ToList();

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

        //categorie
        public IActionResult Categories()
        {
            var category = _dbContext.categories.ToList();

            var viewModel = new CategoryViewModel();

            viewModel._categories = category;

            return View(viewModel);
        }

        public async Task<IActionResult> addCategory(CategoryViewModel category)
        {
            await _recipesService.addCategory(category);
            return Redirect("Categories");
        }

        public async Task<IActionResult> lockCategory(CategoryViewModel category)
        {
            await _recipesService.lockCategory(category);
            return Redirect("Categories");
        }

        public async Task<IActionResult> unlockCategory(CategoryViewModel category)
        {
            await _recipesService.unlockCategory(category);
            return Redirect("Categories");
        }

        public async Task<IActionResult> deleteCategory(CategoryViewModel category)
        {
            await _recipesService.deleteCategory(category);
            return Redirect("Categories");
        }
    }
}
