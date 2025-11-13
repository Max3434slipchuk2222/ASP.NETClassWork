namespace _1.WorkingMVC.Interfaces;

public interface IEmailService
{
	Task SendEmailAsync(string email, string subject, string htmlMessage);
}