using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var smtpServer = _config["Email:SmtpServer"];
        var smtpPort = _config.GetValue<int>("Email:SmtpPort");
        var smtpUser = _config["Email:SmtpUser"];
        var smtpPass = _config["Email:SmtpPass"];

        if (string.IsNullOrEmpty(smtpServer) || smtpPort == 0 ||
            string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass))
        {
            throw new InvalidOperationException("Faltan valores en la configuración de Email.");
        }

        using var client = new SmtpClient(smtpServer, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUser, smtpPass),
            EnableSsl = true
        };

        var mailMessage = new MailMessage(smtpUser, to, subject, body)
        {
            IsBodyHtml = true
        };

        try
        {
            await client.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            // Aquí verás el error real si falla
            Console.WriteLine($"Error enviando correo: {ex.Message}");
            throw;
        }
    }
}