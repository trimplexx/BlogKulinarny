using BlogKulinarny.Data.Services;
using BlogKulinarny.Models;
using BlogKulinarny.Models.AuthModels;
using BlogKulinarny.Models.UserModels;
using Microsoft.AspNetCore.Mvc;

namespace BlogKulinarny.Controllers;

/// <summary>
///     Kontroler do obsługi uwierzytelniania użytkowników.
/// </summary>
public class AuthController : Controller
{
    private readonly AuthService _authService;

    /// <summary>
    ///     Tworzy nową instancję kontrolera AuthController.
    /// </summary>
    /// <param name="authService">Serwis uwierzytelniania.</param>
    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    ///     Wyświetla formularz logowania.
    /// </summary>
    [HttpGet]
    public IActionResult Login()
    {
        if (UserIsLoggedIn())
        {
            // Wiadomość dla użytkownika wyświetlana na stronie
            TempData["NotificationMessageType"] = "error";
            TempData["NotificationMessage"] = "Jesteś już zalogowany.";
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    /// <summary>
    ///     Przetwarza formularz logowania.
    /// </summary>
    /// <param name="model">Model danych logowania.</param>
    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        var isAuthenticated = _authService.Login(model.EmailOrLogin, model.Password);

        if (isAuthenticated)
        {
            TempData["UserLoggedInMessageType"] = "success";
            TempData["UserLoggedInMessage"] = "Pomyślnie zalogowano";
            return RedirectToAction("Index", "Home");
        }

        TempData["NotificationMessageType"] = "error";
        TempData["NotificationMessage"] = "Wprowadzono błędne dane!";

        return View(model);
    }

    /// <summary>
    ///     Obsługuje żądanie wylogowania.
    /// </summary>
    public async Task<IActionResult> Logout()
    {
        await _authService.Logout(); // Wywołanie metody Logout w AuthService
        return RedirectToAction("Index", "Home"); // Przekierowanie użytkownika do strony głównej
    }

    /// <summary>
    ///     Wyświetla formularz rejestracji.
    /// </summary>
    public IActionResult Register()
    {
        if (UserIsLoggedIn())
        {
            TempData["NotificationMessageType"] = "error";
            TempData["NotificationMessage"] = "Jesteś już zalogowany.";
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    /// <summary>
    ///     Przetwarza formularz rejestracji.
    /// </summary>
    /// <param name="model">Model danych rejestracji.</param>
    [HttpPost]
    public async Task<IActionResult> Register(RegistrationViewModel model)
    {
        if (ModelState.IsValid)
        {
            var registrationResult =
                await _authService.RegisterUserAsync(model.Login, model.Password, model.Email, this);

            if (registrationResult.Success)
            {
                TempData["NotificationMessageType"] = "success";
                TempData["NotificationMessage"] = "Rejestracja przebiegła pomyślnie. Potwierdź swoje konto na mail.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", registrationResult.ErrorMessage);
        }

        return View("Register", model);
    }

    /// <summary>
    ///     Weryfikuje i resetuje hasła.
    /// </summary>
    /// <param name="token">Token weryfikacyjny.</param>
    [HttpGet]
    public async Task<IActionResult> Verify([FromQuery(Name = "token")] string token)
    {
        try
        {
            await _authService.Verify(token);
            Console.WriteLine("Weryfikacja smiga");
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            var errorViewModel = new ErrorViewModel
            {
                Message = ex.Message
            };

            return View("Error", errorViewModel);
        }
    }

    /// <summary>
    ///     Wyświetla formularz resetowania hasła.
    /// </summary>
    public IActionResult ResetPassword()
    {
        return View();
    }

    /// <summary>
    ///     Wyświetla formularz zmiany hasła.
    /// </summary>
    public IActionResult ChangePassword()
    {
        return View();
    }

    /// <summary>
    ///     Wysyła link do resetowania hasła.
    /// </summary>
    /// <param name="model">Model danych resetowania hasła.</param>
    public async Task<IActionResult> SendPasswdLink(ResetPasswordViewModel model)
    {
        try
        {
            await _authService.SendPasswdLink(model.Email, this);
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            var errorViewModel = new ErrorViewModel
            {
                Message = ex.Message
            };

            return View("Error", errorViewModel);
        }
    }

    /// <summary>
    ///     Zmienia hasło użytkownika.
    /// </summary>
    /// <param name="token">Token zmiany hasła.</param>
    /// <param name="model">Model danych zmiany hasła.</param>
    public async Task<IActionResult> ChangePasswd([FromQuery(Name = "token")] string token,
        ChangePasswordViewModel model)
    {
        try
        {
            await _authService.ChangePasswd(token, model.Password);
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            var errorViewModel = new ErrorViewModel
            {
                Message = ex.Message
            };

            return View("Error", errorViewModel);
        }
    }

    /// <summary>
    ///     Sprawdza, czy użytkownik jest zalogowany.
    /// </summary>
    /// <returns>Wartość true, jeśli użytkownik jest zalogowany. W przeciwnym razie wartość false.</returns>
    private bool UserIsLoggedIn()
    {
        return HttpContext.Session.TryGetValue("UserId", out _);
    }
}