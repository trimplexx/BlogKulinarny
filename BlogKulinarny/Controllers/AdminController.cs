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
        private readonly AppDbContext _dbContext;

        public AdminController(AdminUsersService UsersService, AppDbContext dbContext) {
            _usersService = UsersService;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UnconfirmedRecipes()
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
                ex.HelpLink = "Error";
                return View(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmUser(AdminUsersViewModel model)
        {
            await _usersService.ConfirmUser(model.userId);
            return Redirect("GetUnconfirmedUsers");
        }

        public async Task<IActionResult> DeleteUser(AdminUsersViewModel model)
        {
            await _usersService.DeleteUser(model.userId);
            return Redirect("GetUnconfirmedUsers");
        }
    }
}
