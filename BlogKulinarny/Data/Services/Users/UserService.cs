using System.Security.Cryptography;
using System.Text;
using BlogKulinarny.Data.Helpers;

namespace BlogKulinarny.Data.Services.Users;

public class UserService
{
    private readonly AppDbContext _dbContext;

    public UserService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<ChangesResult> ChangePasswordAsync(string login, string oldPassword, string newPassword, string confirmNewPassword)
    {
        try
        {
            // Wyszukanie użytkownika w bazie danych
            var user = _dbContext.users.FirstOrDefault(u => u.login == login);
            if (user == null)
                return new ChangesResult(false, "Użytkownik nie istnieje.");

            // Sprawdzenie poprawności starego hasła
            if (user.password != HashPassword(oldPassword))
                return new ChangesResult(false, "Podane stare hasło jest nieprawidłowe.");

            // Sprawdzenie czy nowe hasło spełnia wymagania
            if (!IsPasswordValid(newPassword))
                return new ChangesResult(false, "Hasło powinno składać się z przynajmniej 8 znaków, dużej litery oraz znaku specjalnego.");

            // Sprawdzenie czy nowe hasło zgadza się z powtórzonym hasłem
            if (newPassword != confirmNewPassword)
                return new ChangesResult(false, "Nowe hasło i powtórzone hasło nie zgadzają się.");

            // Sprawdzenie czy nowe hasło nie jest takie samo jak stare
            if (newPassword == oldPassword)
                return new ChangesResult(false, "Nowe hasło nie może być takie samo jak stare hasło.");

            // Aktualizacja hasła
            user.password = HashPassword(newPassword);
            await _dbContext.SaveChangesAsync();

            return new ChangesResult(true, "Zmiana hasła przebiegła pomyślnie.");
        }
        catch (Exception)
        {
            // Obsługa wyjątku - np. zapisanie w logach, wyrzucenie odpowiedniego komunikatu itp.
            return new ChangesResult(false, "Wystąpił błąd podczas zmiany hasła.");
        }
    }

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

    private static string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}