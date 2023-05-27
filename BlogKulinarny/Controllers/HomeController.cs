using BlogKulinarny.Data.Services.Admin;
using BlogKulinarny.Data;
using BlogKulinarny.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace BlogKulinarny.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AdminUsersService _usersService;
        private readonly AppDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, AppDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult RecipesList(string SearchForRecipe)
        {
            var recipes = _dbContext.recipes.Include(r => r.recipesCategories).ThenInclude(rc => rc.category).Where(r => r.isAccepted == true).ToList();
            try
            {
                if (string.IsNullOrWhiteSpace(SearchForRecipe))
                {
                    return View(recipes);
                }
                recipes = _dbContext.recipes
                    .Include(r => r.recipesCategories)
                    .ThenInclude(rc => rc.category)
                    .Where(r => r.isAccepted == true && r.title
                    .Contains(SearchForRecipe) || r.recipesCategories
                    .Any(rc => rc.category.name
                    .Contains(SearchForRecipe)))
                    .ToList();

                //Console.Write(recipes.recipesCategories);
                return View(recipes);
            }
            catch (Exception ex)
            {
                var errorModel = new ErrorViewModel
                {
                    //Message = "An error occurred while retrieving recipes.",
                    //Exception = ex
                };
                return View("Error", ex);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}