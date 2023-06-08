using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BlogKulinarny.Data.Helpers;

/// <summary>
///     Klasa filtru uprawnień dla kontrolowania dostępu do akcji na podstawie rangi użytkownika.
/// </summary>
public class AuthorizeRankFilter : ActionFilterAttribute
{
    private readonly AppDbContext _dbContext;
    private readonly int _minimumRequiredRank;

    /// <summary>
    ///     Inicjalizuje nową instancję klasy AuthorizeRankFilter z domyślną minimalną rangą.
    /// </summary>
    /// <param name="dbContext">Kontekst bazy danych aplikacji.</param>
    public AuthorizeRankFilter(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    ///     Inicjalizuje nową instancję klasy AuthorizeRankFilter z określoną minimalną rangą.
    /// </summary>
    /// <param name="minimumRequiredRank">Wymagana minimalna ranga dla dostępu do akcji.</param>
    /// <param name="dbContext">Kontekst bazy danych aplikacji.</param>
    public AuthorizeRankFilter(int minimumRequiredRank, AppDbContext dbContext)
    {
        _minimumRequiredRank = minimumRequiredRank;
        _dbContext = dbContext;
    }

    /// <summary>
    ///     Przesłania działanie przed wykonaniem metody kontrolera, sprawdzając rangę użytkownika.
    /// </summary>
    /// <param name="context">Kontekst wykonania akcji.</param>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        int? userRank = null;
        // Spróbuj pobrać wartość "UserId" z sesji
        if (context.HttpContext.Session.TryGetValue("UserId", out var userIdBytes)
            && int.TryParse(Encoding.UTF8.GetString(userIdBytes), out var userId))
        {
            // Wyszukaj użytkownika o podanym Id w bazie danych
            var user = _dbContext.users.FirstOrDefault(u => u.Id == userId);
            if (user != null) userRank = (int?)user.rank;
        }

        // Sprawdź, czy ranga użytkownika spełnia wymagania
        if (userRank.HasValue && userRank.Value >= _minimumRequiredRank)
            base.OnActionExecuting(context);
        else
            // Przekieruj do akcji "NoAccess" kontrolera "Home"
            context.Result = new RedirectToActionResult("NoAccess", "Home", null);
    }
}