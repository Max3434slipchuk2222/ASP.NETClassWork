using _1.WorkingMVC.Interfaces;
using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace _1.WorkingMVC.Services;

public class EmailService : IEmailService
{
	private readonly IConfiguration _configuration;

	public EmailService(IConfiguration configuration)
	{
		_configuration = configuration;
	}
	public async Task SendEmailAsync(string email, string subject, string htmlMessage)
	{
		var SmtpHost = _configuration["Smtp:Host"];
		var SmtpPort = _configuration.GetValue<int>("Smtp:Port");
		var SmtpUser = _configuration["Smtp:User"];
		var SmtpPass = _configuration["Smtp:Password"];
		var message = new MimeMessage();
		message.From.Add(new MailboxAddress("ASP.NET MVC Project", SmtpUser));
		message.To.Add(new MailboxAddress("", email));
		message.Subject = subject;
		message.Body = new TextPart("html") { Text = htmlMessage };
		using (var client = new SmtpClient())
		{
			try
			{ 
			await client.ConnectAsync(SmtpHost, SmtpPort, SecureSocketOptions.StartTls);
				await client.AuthenticateAsync(SmtpUser, SmtpPass);
				await client.SendAsync(message);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Помилка: {ex.Message}");
			}
			finally
			{
				await client.DisconnectAsync(true);
			}
		}
	}
}