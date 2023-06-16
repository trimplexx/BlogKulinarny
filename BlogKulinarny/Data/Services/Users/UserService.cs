using System.Security.Cryptography;
using System.Text;
using BlogKulinarny.Data.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BlogKulinarny.Data.Services.Users;

/// <summary>
///     Klasa serwisu użytkowników
/// </summary>
public class UserService
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    ///     Inicjalizuje nową instancję klasy <see cref="UserService" />.
    /// </summary>
    /// <param name="dbContext">Kontekst bazy danych.</param>
    /// <param name="httpContextAccessor">Dostęp do kontekstu HTTP.</param>
    public UserService(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    ///     Usuwa konto użytkownika.
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika.</param>
    /// <param name="password">Hasło użytkownika.</param>
    /// <returns>Wynik operacji.</returns>
    public async Task<bool> DeleteAccountAsync(string userId, string password)
    {
        int userIdAsInt;

        // Spróbuj przekonwertować wartość userId na typ int
        if (!int.TryParse(userId, out userIdAsInt)) return false; // Jeśli konwersja się nie powiedzie, zwróć false

        // Użyj przekonwertowanego userIdAsInt zamiast userId
        var user = await _dbContext.users.FindAsync(userIdAsInt);
        if (user != null && user.password == HashPassword(password))
        {
            _dbContext.users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Zmienia hasło użytkownika.
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika.</param>
    /// <param name="oldPassword">Stare hasło użytkownika.</param>
    /// <param name="newPassword">Nowe hasło użytkownika.</param>
    /// <returns>Wynik operacji.</returns>
    public async Task<ChangesResult> ChangePasswordAsync(string userId, string oldPassword, string newPassword)
    {
        int userIdAsInt;

        // Spróbuj przekonwertować wartość userId na typ int
        if (!int.TryParse(userId, out userIdAsInt)) return new ChangesResult(false, "Błąd konwersji Id");

        // Użyj przekonwertowanego userIdAsInt zamiast userId
        var user = await _dbContext.users.FindAsync(userIdAsInt);
        if (user != null)
        {
            var oldPasswordHash = HashPassword(oldPassword);
            if (user.password == oldPasswordHash)
            {
                if (IsPasswordValid(newPassword))
                {
                    var newPasswordHash = HashPassword(newPassword);
                    if (user.password != newPasswordHash)
                    {
                        user.password = newPasswordHash;
                        _dbContext.users.Update(user);
                        await _dbContext.SaveChangesAsync();
                        return new ChangesResult(true, "Poprawnie zmieniono hasło");
                    }

                    return new ChangesResult(false, "Nowe hasło musi być różne od starego hasła.");
                }

                return new ChangesResult(false, "Hasło nie spełnia wymagań.");
            }
        }

        return new ChangesResult(false, "Nie udało się zmienić hasła.");
    }

    /// <summary>
    ///     Generuje skrót hasła.
    /// </summary>
    /// <param name="password">Hasło do zahashowania.</param>
    /// <returns>Skrót hasła.</returns>
    private static string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    /// <summary>
    ///     Sprawdza, czy hasło jest poprawne.
    /// </summary>
    /// <param name="password">Hasło do sprawdzenia.</param>
    /// <returns>Wynik walidacji hasła.</returns>
    private bool IsPasswordValid(string password)
    {
        // Sprawdzenie czy hasło spełnia wymagania: przynajmniej 8 znaków, duża litera i znak specjalny
        if ((password.Length < 8 && password.Length > 40) || !password.Any(char.IsUpper)
                                                          || !password.Any(IsSpecialCharacter)) return false;

        return true;
    }

    /// <summary>
    ///     Sprawdza, czy znak jest znakiem specjalnym.
    /// </summary>
    /// <param name="c">Znak do sprawdzenia.</param>
    /// <returns>Wynik sprawdzenia.</returns>
    private bool IsSpecialCharacter(char c)
    {
        return !char.IsLetterOrDigit(c);
    }

    /// <summary>
    ///     Aktualizuje adres email użytkownika.
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika.</param>
    /// <param name="newEmail">Nowy adres email.</param>
    /// <returns>Wynik operacji.</returns>
    public async Task<bool> UpdateUserEmailAsync(string userId, string? newEmail)
    {
        int userIdAsInt;

        // Spróbuj przekonwertować wartość userId na typ int
        if (!int.TryParse(userId, out userIdAsInt)) return false; // Jeśli konwersja się nie powiedzie, zwróć false

        var user = await _dbContext.users.FirstOrDefaultAsync(u => u.Id == userIdAsInt);
        if (user == null) return false;

        // Sprawdź, czy nowy adres email różni się od aktualnego adresu email
        if (user.mail != newEmail)
        {
            user.mail = newEmail;
            await _dbContext.SaveChangesAsync();
            _httpContextAccessor.HttpContext?.Session.SetString("Email", newEmail);
            return true;
        }

        // Jeśli adresy email są takie same, zwróć false, aby oznaczyć brak zmian
        return false;
    }
}