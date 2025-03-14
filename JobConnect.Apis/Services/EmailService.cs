using JobConnect.Core.Models;
using JobConnect.Core.Services;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Net.Mail;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace JobConnect.Services
{
	public class EmailService : IEmailService
	{
		private readonly EmailSettings _emailSettings;

		public EmailService(IOptions<EmailSettings> emailSettings)
		{
			_emailSettings = emailSettings.Value;
		}

		public async Task SendEmailAsync(string toEmail, string subject, string body)
		{
			var emailMessage = new MimeMessage();
			emailMessage.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
			emailMessage.To.Add(new MailboxAddress("", toEmail)); 

			emailMessage.Subject = subject;

			var bodyBuilder = new BodyBuilder { HtmlBody = body };
			emailMessage.Body = bodyBuilder.ToMessageBody();

			using (var smtpClient = new SmtpClient())
			{
				await smtpClient.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, false);
				await smtpClient.AuthenticateAsync(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
				await smtpClient.SendAsync(emailMessage);
				await smtpClient.DisconnectAsync(true);
			}
		}
	}
}
