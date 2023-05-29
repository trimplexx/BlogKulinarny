using BlogKulinarny.Data.Services;
using BlogKulinarny.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogKulinarny.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {

            bool isAuthenticated = _authService.Login(model.EmailOrLogin, model.Password);

            if (isAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Nieprawidłowy adres email, login lub hasło.");

            return View(model);
        }
        
        // Metoda obsługująca żądanie wylogowania
        public async Task<IActionResult> Logout()
        {
            await _authService.Logout(); // Wywołanie metody Logout w AuthService
            return RedirectToAction("Index", "Home"); // Przekierowanie użytkownika do strony głównej
        }

        public IActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Register(RegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var registrationResult = await _authService.RegisterUserAsync(model.Login, model.Password, model.Email, this);

                if (registrationResult.Success)
                {
                    TempData["RegistrationSuccess"] = "Rejestracja przebiegła pomyślnie. Możesz się teraz zalogować.";
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", registrationResult.ErrorMessage);
                }
            }

            return View("Register", model);
        }

        /// <summary>
        /// weryfikacje i resetowanie hasel
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Verify([FromQuery(Name = "token")] string token)
        {
            var verifyResult = await _authService.Verify(token);
            Console.WriteLine("Weryfikacja smiga");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ResetPassword()
        {
            return View();
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        public async Task<IActionResult> SendPasswdLink(ResetPasswordViewModel model)
        {
            var res = await _authService.SendPasswdLink(model.Email, this);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> ChangePasswd([FromQuery(Name = "token")] string token,ChangePasswordViewModel model)
        {
            var verifyResult = await _authService.ChangePasswd(token,model.Password);
            return RedirectToAction("Index", "Home");
        }
    }
}
