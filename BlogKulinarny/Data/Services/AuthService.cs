using System.Security.Cryptography;
using System.Text;
using BlogKulinarny.Data.Enums;
using BlogKulinarny.Data.Helpers;
using BlogKulinarny.Data.Services.Admin;
using BlogKulinarny.Models;
using Microsoft.AspNetCore.Authentication;

namespace BlogKulinarny.Data.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public AuthService(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Logowanie
    /// </summary>
    public bool Login(string emailOrLogin, string password)
    {
        string hashedPassword = HashPassword(password);

        User? user = _dbContext.users.FirstOrDefault(u => (u.mail == emailOrLogin || u.login == emailOrLogin) && u.password == hashedPassword);

        if (user != null)
        if (user == null)
        {
            // brak wgl podanych danych
        {
            // Ustaw sesję użytkownika
            _httpContextAccessor.HttpContext.Session.SetString("UserId", user.Id.ToString());
            _httpContextAccessor.HttpContext.Session.SetString("Login", user.login);
            return true;
        }
            // Utwórz sesję użytkownika lub zapisz informacje o zalogowanym użytkowniku w sesji
            // Na przykład:
            //HttpContext.Session.SetString("UserId", user.Id.ToString());
            return false;
        }
        if(VerifyPassword(password, user.password) == false || user.isAccepted == 0)
        {
            // zle haslo ablo nie aktywowane konto
            return false;
        }

        return true;
    }

    public async Task Logout()
    {
        if (_httpContextAccessor.HttpContext != null)
        {
            await _httpContextAccessor.HttpContext.SignOutAsync(); // Wylogowanie użytkownika

            // Wyczyść dane sesji
            _httpContextAccessor.HttpContext.Session.Clear();
            _httpContextAccessor.HttpContext.Session.Remove("UserId");
            _httpContextAccessor.HttpContext.Session.Remove("Login");
        }
    }

    /// <summary>
    /// Metody związane z logowaniem
    /// </summary>
    private static string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    private static bool VerifyPassword(string enteredPassword, string hashedPassword)
    {
        byte[] hashedBytes = Convert.FromBase64String(hashedPassword);
        using var sha256 = SHA256.Create();
        byte[] enteredBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(enteredPassword));
        return hashedBytes.SequenceEqual(enteredBytes);
    }

    /// <summary>
    /// Rejestracja
    /// </summary>
    public async Task<ChangesResult> RegisterUserAsync(string login, string password, string email)
    {
        try
        {
            // Walidacja długości loginu, maila i hasła
            if (login.Length > 40 || password.Length > 40 || email.Length > 40)
                return new ChangesResult(false, "Maksymalna długość loginu, hasła i adresu email to 40 znaków.");
            
            // Sprawdzenie czy podany adres email został już użyty
            if (_dbContext.users.Any(u => u.mail == email))
                return new ChangesResult(false, "Podany adres email został już użyty.");

            // Sprawdzenie czy podany login został już użyty
            if (_dbContext.users.Any(u => u.login == login))
                return new ChangesResult(false, "Podany login został już użyty.");

            // Sprawdzenie czy hasło spełnia wymagania
            if (!IsPasswordValid(password))
                return new ChangesResult(false,
                    "Hasło powinno składać się z przynajmniej 8 znaków, dużej litery oraz znaku specjalnego.");

            // Tworzenie nowego użytkownika
            var newUser = new User
            {
                login = login,
                password = HashPassword(password),
                mail = email,
                isAccepted = 0,
                rank = Ranks.user // Dodawanie nowej roli
            };

            // Dodanie użytkownika do bazy danych
            _dbContext.users.Add(newUser);
            await _dbContext.SaveChangesAsync();

            return new ChangesResult(true, "Rejestracja przebiegła pomyślnie.");
        }
        catch (Exception)
        {
            // Obsługa wyjątku - np. zapisanie w logach, wyrzucenie odpowiedniego komunikatu itp.
            return new ChangesResult(false, "Wystąpił błąd podczas rejestracji użytkownika.");
        }
    }

    /// <summary>
    /// Metody związane z rejestracja
    /// </summary>
    private bool IsPasswordValid(string password)
    {
        // Sprawdzenie czy hasło spełnia wymagania: przynajmniej 8 znaków, duża litera i znak specjalny
        if (password.Length < 8 && password.Length > 40 || !password.Any(char.IsUpper) || !password.Any(IsSpecialCharacter)) return false;

        return true;
    }

    private bool IsSpecialCharacter(char c)
    {
        // Sprawdzenie czy znak jest znakiem specjalnym
        return !char.IsLetterOrDigit(c);
    }

}