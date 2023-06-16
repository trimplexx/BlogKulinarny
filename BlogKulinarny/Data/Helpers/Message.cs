using MimeKit;

namespace BlogKulinarny.Data.Helpers;

/// <summary>
///     Klasa reprezentująca wiadomość e-mail w projekcie bloga kulinarnego.
/// </summary>
public class Message
{
    /// <summary>
    ///     Inicjalizuje nową instancję klasy Message z określonymi wartościami dla odbiorców, tematu i treści.
    /// </summary>
    /// <param name="to">Kolekcja adresów e-mail odbiorców wiadomości.</param>
    /// <param name="subject">Temat wiadomości e-mail.</param>
    /// <param name="content">Treść wiadomości e-mail.</param>
    public Message(IEnumerable<string> to, string subject, string content)
    {
        To = new List<MailboxAddress>();
        // Dodaj adresy e-mail odbiorców do listy
        To.AddRange(to.Select(x => new MailboxAddress("informatykaelektryks3@gmail.com", x)));

        Subject = subject;
        Content = content;
    }

    /// <summary>
    ///     Lista adresów e-mail odbiorców wiadomości.
    /// </summary>
    public List<MailboxAddress> To { get; set; }

    /// <summary>
    ///     Temat wiadomości e-mail.
    /// </summary>
    public string Subject { get; set; }

    /// <summary>
    ///     Treść wiadomości e-mail.
    /// </summary>
    public string Content { get; set; }
}