using BlogKulinarny.Data.Services;
using BlogKulinarny.Models;
using Microsoft.AspNetCore.Mvc;

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
            if (ModelState.IsValid)
            {
                bool isAuthenticated = _authService.Login(model.EmailOrLogin, model.Password);

                if (isAuthenticated)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Nieprawidłowy adres email, login lub hasło.");
            }

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
                var registrationResult = await _authService.RegisterUserAsync(model.Login, model.Password, model.Email);

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
    }
}
