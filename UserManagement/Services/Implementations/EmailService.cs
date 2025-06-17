using System.Net;
using System.Net.Mail;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services.Implementations;

public class EmailService: IEmailService
{
        public async Task   SendPasswordResetEmail(string email,string resetLink)
    {
        try
        {
            
            Console.WriteLine("Email link send");

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("jigneshbambhava111@gmail.com", "wvyu jzjk xegr jzak"),
                EnableSsl = true,
            };

            string emailTemplate = await File.ReadAllTextAsync("Views/Email/ResetPassword.cshtml");
            string emailBody = emailTemplate.Replace("{{ResetLink}}", resetLink);

            var mailMessage = new MailMessage
            {
                From = new MailAddress("test.dotnet@etatvasoft.com"),
                Subject = "Reset Your Password",
                Body = emailBody,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);

            Console.WriteLine("Email link end");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
        }
    }

    public async Task SendAccountDetailsEmail(string email, string username, string temporaryPassword)
    {
        try
        {
            Console.WriteLine("Email link send");

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("jigneshbambhava111@gmail.com", "wvyu jzjk xegr jzak"),
                EnableSsl = true,
            };

            string emailTemplate = await File.ReadAllTextAsync("Views/Email/LoginDetails.cshtml");
            string emailBody = emailTemplate
                    .Replace("{{Username}}", username)
                    .Replace("{{TemporaryPassword}}", temporaryPassword);

            var mailMessage = new MailMessage
            {
                From = new MailAddress("test.dotnet@etatvasoft.com"),
                Subject = "Reset Your Password",
                Body = emailBody,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);

            Console.WriteLine("Email link end");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
        }
    }
}
