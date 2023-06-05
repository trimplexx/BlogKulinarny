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

        public IActionResult RecipesList(string SearchForRecipe, string sortOption1, string sortOption2, string sortOption3, string unlock)
        {
            ViewBag.SearchForRecipe = SearchForRecipe;
            try
            {
                var recipes = _dbContext.recipes
                    .Include(r => r.recipesCategories)
                    .ThenInclude(rc => rc.category)
                    .Where(r => r.isAccepted == true)
                    .OrderByDescending(r => r.id)
                    .ToList();

                if (!string.IsNullOrWhiteSpace(SearchForRecipe))
                {
                    recipes = _dbContext.recipes
                        .Include(r => r.recipesCategories)
                        .ThenInclude(rc => rc.category)
                        .Where(r => r.isAccepted == true && r.title
                            .Contains(SearchForRecipe) || r.recipesCategories
                            .Any(rc => rc.category.name
                                .Contains(SearchForRecipe)))
                        .OrderByDescending(r => r.id)
                        .ToList();
                }

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
                    Message = ex.Message,
                };

                return View("Error", errorViewModel);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult NoAccess()
        {
            return View();
        }
    }
}