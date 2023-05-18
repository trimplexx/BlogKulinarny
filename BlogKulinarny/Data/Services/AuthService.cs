using System.Security.Cryptography;
using System.Text;
using BlogKulinarny.Data.Enums;
using BlogKulinarny.Models;

namespace BlogKulinarny.Data.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;

    public AuthService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    /// <summary>
    /// Logowanie
    /// </summary>
    /// <param name="emailOrLogin"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public bool Login(string emailOrLogin, string password)
    {
        var user = _dbContext.users.FirstOrDefault(u =>
            (u.mail == emailOrLogin || u.login == emailOrLogin) && u.password == password);

        if (user != null)
            // Utwórz sesję użytkownika lub zapisz informacje o zalogowanym użytkowniku w sesji
            // Na przykład:
            // HttpContext.Session.SetString("UserId", user.Id.ToString());
            return true;

        return false;
    }

    /// <summary>
    /// Rejestracja
    /// </summary>
    /// <param name="login"></param>
    /// <param name="password"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task<RegistrationResult> RegisterUserAsync(string login, string password, string email)
    {
        try
        {
            // Walidacja długości loginu, maila i hasła
            if (login.Length > 40 || password.Length > 40 || email.Length > 40)
                return new RegistrationResult(false, "Maksymalna długość loginu, hasła i adresu email to 40 znaków.");
            
            // Sprawdzenie czy podany adres email został już użyty
            if (_dbContext.users.Any(u => u.mail == email))
                return new RegistrationResult(false, "Podany adres email został już użyty.");

            // Sprawdzenie czy podany login został już użyty
            if (_dbContext.users.Any(u => u.login == login))
                return new RegistrationResult(false, "Podany login został już użyty.");

            // Sprawdzenie czy hasło spełnia wymagania
            if (!IsPasswordValid(password))
                return new RegistrationResult(false,
                    "Hasło powinno składać się z przynajmniej 8 znaków, dużej litery oraz znaku specjalnego.");

            // Tworzenie nowego użytkownika
            var newUser = new User
            {
                login = login,
                password = HashPassword(password),
                mail = email,
                isAccepted = false,
                rank = Ranks.user // Dodawanie nowej roli
            };

            // Dodanie użytkownika do bazy danych
            _dbContext.users.Add(newUser);
            await _dbContext.SaveChangesAsync();

            return new RegistrationResult(true, "Rejestracja przebiegła pomyślnie.");
        }
        catch (Exception ex)
        {
            // Obsługa wyjątku - np. zapisanie w logach, wyrzucenie odpowiedniego komunikatu itp.
            return new RegistrationResult(false, "Wystąpił błąd podczas rejestracji użytkownika.");
        }
    }

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

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    public class RegistrationResult
    {
        public RegistrationResult(bool success, string errorMessage)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }

        public bool Success { get; }
        public string ErrorMessage { get; }
    }
}