using Microsoft.AspNetCore.Mvc;

namespace BlogKulinarny.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ConfirmUsers()
        {
            return View();
        }

        public IActionResult ConfirmRecipes()
        {
            return View();
        }
    }
}
