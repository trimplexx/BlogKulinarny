using System.Security.Cryptography;
using System.Text;
using BlogKulinarny.Data.Enums;
using BlogKulinarny.Data.Helpers;
using BlogKulinarny.Data.Services.Admin;
using BlogKulinarny.Data.Services.Mail;
using BlogKulinarny.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using BlogKulinarny.Data.Helpers;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.EntityFrameworkCore;

namespace BlogKulinarny.Data.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public AuthService(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor,IEmailSender emailSender)
    {
        _emailSender = emailSender;
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Logowanie
    /// </summary>
    public bool Login(string emailOrLogin, string password)
    {
        string hashedPassword = HashPassword(password);

        User user = _dbContext.users.FirstOrDefault(u => (u.mail == emailOrLogin || u.login == emailOrLogin) && u.password == hashedPassword);

        if (user == null)
        {
            return false;
        }

        if (VerifyPassword(password, user.password) == false || user.isAccepted == false)
        {
            return false;
        }

        // Ustaw sesję użytkownika
        _httpContextAccessor.HttpContext.Session.SetString("UserId", user.Id.ToString());
        _httpContextAccessor.HttpContext.Session.SetString("Login", user.login);

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
    public async Task<ChangesResult> RegisterUserAsync(string login, string password, string email, Controller controller)
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
                VerificationToken = CreateRandomToken()
            };

            string link = $"{Utilities.GetBuilder(controller).Uri.AbsoluteUri}Auth/verify?token={newUser.VerificationToken}";

            Message verification = new Message(new string[] {newUser.mail},
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
    /// Metody związane z rejestracja
    /// </summary>
    private bool IsPasswordValid(string password)
    {
        // Sprawdzenie czy hasło spełnia wymagania: przynajmniej 8 znaków, duża litera i znak specjalny
        if (password.Length < 8 && password.Length > 40 || !password.Any(char.IsUpper) 
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
    /// Metody związane z weryfikacja konta
    /// </summary>
    public async Task<ChangesResult> Verify(string token)
    {
        User user = await _dbContext.users.FirstOrDefaultAsync(u => (u.VerificationToken == token));

        if (user == null)
            return new ChangesResult(false, "Nieprawidlowy token.");

        if (user.isAccepted)
            return new ChangesResult(false, "Konto juz zostało zaaktywowane");

        user.VerifiedAt = DateTime.Now;
        user.isAccepted = true;
        await _dbContext.SaveChangesAsync();
        return new ChangesResult(true, "Weryfikacja przebiegła pomyślnie.");
    }

    public async Task<ChangesResult> SendPasswdLink(string email, Controller controller)
    {
        User user = await _dbContext.users.FirstOrDefaultAsync(u => (u.mail == email));

        if (user == null)
            return new ChangesResult(false, "brak maila w bazie");

        user.PasswordResetToken = CreateRandomToken();
        user.ResetTokenExpires = DateTime.Now.AddMinutes(15);

        string link = $"{Utilities.GetBuilder(controller).Uri.AbsoluteUri}Auth/ChangePassword?token={user.PasswordResetToken}";

        Message changeMail = new Message(new string[] { user.mail },
            "Resetowanie Hasla", "Tutaj jest link do zmiany hasla:\n" + link + "\n link będzie aktywny przez 15 minut");
        SendTokenMail(changeMail);

        await _dbContext.SaveChangesAsync();
        return new ChangesResult(true, "Wyslano link z resetowaniem hasla");
    }

    public async Task<ChangesResult> ChangePasswd(string token, string password)
    {
        User user = await _dbContext.users.FirstOrDefaultAsync(u => (u.PasswordResetToken == token));

        if (user == null)
            return new ChangesResult(false, "brak maila w bazie");

        if (user.ResetTokenExpires < DateTime.Now)
        {
            return new ChangesResult(false, "link wygasl");
        }

        user.password = HashPassword(password);
        user.ResetTokenExpires = null;
        user.PasswordResetToken = null;

        await _dbContext.SaveChangesAsync();
        return new ChangesResult(true, "Wyslano link z resetowaniem hasla");
    }
    /// <summary>
    /// Metody związane z mailami
    /// </summary>
    private readonly IEmailSender _emailSender;

    public void SendTokenMail(Message message)
    {
        _emailSender.SendEmail(message);
    }
}