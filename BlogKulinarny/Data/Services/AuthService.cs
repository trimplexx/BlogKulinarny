using System.Security.Cryptography;
using System.Text;
using BlogKulinarny.Data.Enums;
using BlogKulinarny.Data.Helpers;
using BlogKulinarny.Data.Services.Mail;
using BlogKulinarny.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogKulinarny.Data.Services;

/// <summary>
///     Serwis odpowiedzialny za uwierzytelnianie użytkowników.
/// </summary>
public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;

    /// <summary>
    ///     Metody związane z mailami
    /// </summary>
    private readonly IEmailSender _emailSender;

    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    ///     Inicjalizuje nową instancję klasy <see cref="AuthService" />.
    /// </summary>
    /// <param name="dbContext">Kontekst bazy danych.</param>
    /// <param name="httpContextAccessor">Dostęp do kontekstu HTTP.</param>
    /// <param name="emailSender">Usługa wysyłania e-maili.</param>
    public AuthService(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor, IEmailSender emailSender)
    {
        _emailSender = emailSender;
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    ///     Loguje użytkownika do systemu.
    /// </summary>
    /// <param name="emailOrLogin">Adres e-mail lub login użytkownika.</param>
    /// <param name="password">Hasło użytkownika.</param>
    /// <returns>Prawda, jeśli logowanie powiodło się; w przeciwnym razie, fałsz.</returns>
    public bool Login(string emailOrLogin, string? password)
    {
        var hashedPassword = HashPassword(password);

        var user = _dbContext.users.FirstOrDefault(u =>
            (u.mail == emailOrLogin || u.login == emailOrLogin) && u.password == hashedPassword);

        if (user == null) return false;

        if (VerifyPassword(password, user.password) == false || user.isAccepted == false) return false;

        // Ustaw sesję użytkownika
        _httpContextAccessor.HttpContext?.Session.SetString("UserId", user.Id.ToString());
        _httpContextAccessor.HttpContext?.Session.SetString("Login", user.login);
        _httpContextAccessor.HttpContext?.Session.SetString("Email", user.mail);
        _httpContextAccessor.HttpContext?.Session.SetString("Avatar", user.imageURL);

        return true;
    }

    /// <summary>
    ///     Wylogowuje użytkownika.
    /// </summary>
    /// <returns>Task reprezentujący asynchroniczne wylogowanie.</returns>
    public async Task Logout()
    {
        if (_httpContextAccessor.HttpContext != null)
        {
            await _httpContextAccessor.HttpContext.SignOutAsync(); // Wylogowanie użytkownika

            // Wyczyść dane sesji
            _httpContextAccessor.HttpContext.Session.Clear();
            _httpContextAccessor.HttpContext.Session.Remove("UserId");
            _httpContextAccessor.HttpContext.Session.Remove("Login");
            _httpContextAccessor.HttpContext.Session.Remove("Email");
            _httpContextAccessor.HttpContext.Session.Remove("Avatar");
        }
    }

    /// <summary>
    ///     Metody związane z logowaniem
    /// </summary>
    private static string HashPassword(string? password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    private static bool VerifyPassword(string? enteredPassword, string hashedPassword)
    {
        var hashedBytes = Convert.FromBase64String(hashedPassword);
        using var sha256 = SHA256.Create();
        var enteredBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(enteredPassword));
        return hashedBytes.SequenceEqual(enteredBytes);
    }

    /// <summary>
    ///     Rejestruje nowego użytkownika.
    /// </summary>
    /// <param name="login">Login użytkownika.</param>
    /// <param name="password">Hasło użytkownika.</param>
    /// <param name="email">Adres e-mail użytkownika.</param>
    /// <param name="controller">Kontroler wywołujący rejestrację.</param>
    /// <returns>Wynik zmiany zawierający informacje o powodzeniu lub niepowodzeniu rejestracji.</returns>
    public async Task<ChangesResult> RegisterUserAsync(string? login, string? password, string? email,
        Controller controller)
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
                isAccepted = false,
                rank = (int)Ranks.user,
                VerificationToken = CreateRandomToken(),
                imageURL = "/images/avatars/avatar0.webp"
            };

            var link =
                $"{Utilities.GetBuilder(controller).Uri.AbsoluteUri}Auth/verify?token={newUser.VerificationToken}";

            var verification = new Message(new[] { newUser.mail },
                "Potwierdzenie Adresu mailowego", "Tutaj jest link aktywacji konta:\n" + link);
            SendTokenMail(verification);

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
    ///     Metody związane z rejestracja
    /// </summary>
    private bool IsPasswordValid(string? password)
    {
        // Sprawdzenie czy hasło spełnia wymagania: przynajmniej 8 znaków, duża litera i znak specjalny
        if ((password.Length < 8 && password.Length > 40) || !password.Any(char.IsUpper)
                                                          || !password.Any(IsSpecialCharacter)) return false;

        return true;
    }

    private bool IsSpecialCharacter(char c)
    {
        return !char.IsLetterOrDigit(c);
    }

    private string CreateRandomToken()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
    }

    /// <summary>
    ///     Weryfikuje konto użytkownika.
    /// </summary>
    /// <param name="token">Token weryfikacji.</param>
    /// <returns>Wynik zmiany zawierający informacje o powodzeniu lub niepowodzeniu weryfikacji.</returns>
    public async Task<ChangesResult> Verify(string token)
    {
        var user = await _dbContext.users.FirstOrDefaultAsync(u => u.VerificationToken == token);

        if (user == null)
            return new ChangesResult(false, "Nieprawidlowy token.");

        if (user.isAccepted)
            return new ChangesResult(false, "Konto juz zostało zaaktywowane");

        user.VerifiedAt = DateTime.Now;
        user.isAccepted = true;
        await _dbContext.SaveChangesAsync();
        return new ChangesResult(true, "Weryfikacja przebiegła pomyślnie.");
    }

    /// <summary>
    ///     Wysyła link do resetowania hasła na adres e-mail użytkownika.
    /// </summary>
    /// <param name="email">Adres e-mail użytkownika.</param>
    /// <param name="controller">Kontroler wywołujący wysyłanie linku.</param>
    /// <returns>Wynik zmiany zawierający informacje o powodzeniu lub niepowodzeniu wysłania linku.</returns>
    public async Task<ChangesResult> SendPasswdLink(string email, Controller controller)
    {
        var user = await _dbContext.users.FirstOrDefaultAsync(u => u.mail == email);

        if (user == null)
            return new ChangesResult(false, "brak maila w bazie");

        user.PasswordResetToken = CreateRandomToken();
        user.ResetTokenExpires = DateTime.Now.AddMinutes(15);

        var link =
            $"{Utilities.GetBuilder(controller).Uri.AbsoluteUri}Auth/ChangePassword?token={user.PasswordResetToken}";

        var changeMail = new Message(new[] { user.mail },
            "Resetowanie Hasla", "Tutaj jest link do zmiany hasla:\n" + link + "\n link będzie aktywny przez 15 minut");
        SendTokenMail(changeMail);

        await _dbContext.SaveChangesAsync();
        return new ChangesResult(true, "Wyslano link z resetowaniem hasla");
    }

    /// <summary>
    ///     Zmienia hasło użytkownika.
    /// </summary>
    /// <param name="token">Token resetowania hasła.</param>
    /// <param name="password">Nowe hasło użytkownika.</param>
    /// <returns>Wynik zmiany zawierający informacje o powodzeniu lub niepowodzeniu zmiany hasła.</returns>
    public async Task<ChangesResult> ChangePasswd(string token, string? password)
    {
        var user = await _dbContext.users.FirstOrDefaultAsync(u => u.PasswordResetToken == token);

        if (user == null)
            return new ChangesResult(false, "brak maila w bazie");

        if (user.ResetTokenExpires < DateTime.Now) return new ChangesResult(false, "link wygasl");

        user.password = HashPassword(password);
        user.ResetTokenExpires = null;
        user.PasswordResetToken = null;

        await _dbContext.SaveChangesAsync();
        return new ChangesResult(true, "Wyslano link z resetowaniem hasla");
    }

    public void SendTokenMail(Message message)
    {
        _emailSender.SendEmail(message);
    }
}