using BlogKulinarny.Data.Helpers;

namespace BlogKulinarny.Data.Services.Mail;

public interface IEmailSender
{
    void SendEmail(Message message);
    Task SendEmailAsync(Message message);
}