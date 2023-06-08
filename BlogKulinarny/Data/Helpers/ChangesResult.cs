namespace BlogKulinarny.Data.Helpers;

/// <summary>
///     Klasa reprezentująca wynik próby wprowadzania zmian w projekcie bloga kulinarnego.
/// </summary>
public class ChangesResult
{
    /// <summary>
    ///     Inicjalizuje nową instancję klasy ChangesResult z określonymi wartościami dla sukcesu i wiadomości błędu.
    /// </summary>
    /// <param name="success">Wartość boolowska wskazująca, czy operacja zakończyła się sukcesem.</param>
    /// <param name="errorMessage">Wiadomość błędu związana z operacją, jeśli wystąpił błąd.</param>
    public ChangesResult(bool success, string errorMessage)
    {
        Success = success;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    ///     Właściwość wskazująca, czy operacja zakończyła się sukcesem.
    /// </summary>
    public bool Success { get; }

    /// <summary>
    ///     Właściwość przechowująca wiadomość błędu związaną z operacją, jeśli wystąpił błąd.
    /// </summary>
    public string ErrorMessage { get; }
}