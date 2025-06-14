namespace UserManagement.Services;

public interface IEmailService
{
    Task SendPasswordResetEmail(string email,string resetLink);
    Task SendAccountDetailsEmail(string email, string username, string temporaryPassword);
}
