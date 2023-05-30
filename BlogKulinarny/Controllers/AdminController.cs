using BlogKulinarny.Data;
using BlogKulinarny.Data.Services.Admin;
using BlogKulinarny.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Exchange.WebServices.Data;

namespace BlogKulinarny.Controllers
{
    public class AdminController : Controller
    {
        private readonly AdminUsersService _usersService;
        private readonly AdminRecipesService _recipesService;
        private readonly AppDbContext _dbContext;

        public AdminController(AdminUsersService UsersService,AdminRecipesService adminRecipesService, AppDbContext dbContext) {
            _usersService = UsersService;
            _dbContext = dbContext;
            _recipesService = adminRecipesService;
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
            return Redirect("UnconfirmedRecipes");
        }

        public IActionResult DetailsUser()
        {
            throw new NotImplementedException();
        }
    }
}
