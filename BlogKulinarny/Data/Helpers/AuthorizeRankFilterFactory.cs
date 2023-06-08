using Microsoft.AspNetCore.Mvc.Filters;

namespace BlogKulinarny.Data.Helpers;

/// <summary>
///     Fabryka filtrów dla tworzenia instancji filtrów AuthorizeRankFilter.
/// </summary>
public class AuthorizeRankFilterFactory : IFilterFactory
{
    private readonly int _minimumRequiredRank;

    /// <summary>
    ///     Inicjalizuje nową instancję klasy AuthorizeRankFilterFactory z określoną minimalną rangą.
    /// </summary>
    /// <param name="minimumRequiredRank">Wymagana minimalna ranga dla dostępu do akcji.</param>
    public AuthorizeRankFilterFactory(int minimumRequiredRank)
    {
        _minimumRequiredRank = minimumRequiredRank;
    }

    /// <summary>
    ///     Właściwość określająca, czy filtr można wielokrotnie używać.
    /// </summary>
    public bool IsReusable => false;

    /// <summary>
    ///     Tworzy instancję filtru AuthorizeRankFilter z odpowiednim kontekstem bazy danych.
    /// </summary>
    /// <param name="serviceProvider">Dostawca usług dla aplikacji.</param>
    /// <returns>Nowa instancja filtru AuthorizeRankFilter.</returns>
    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        // Pobierz kontekst bazy danych z dostawcy usług
        var dbContext = serviceProvider.GetRequiredService<AppDbContext>();
        // Zwróć nową instancję filtru AuthorizeRankFilter z określoną minimalną rangą i kontekstem bazy danych
        return new AuthorizeRankFilter(_minimumRequiredRank, dbContext);
    }
}