using BusinessLogic.Configurations;
using BusinessLogic.Dtos.Contact;
using BusinessLogic.Dtos.Mail;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BusinessLogic.Services.Common
{
    public interface IEmailService
    {
        Task SendAsync(MailRequest request);

        Task SendMultiAsync(MailMultiRequest request);

        Task SendContactAsync(ContactRequest request);
    }

    public class EmailService(IOptions<MailConfiguration> mailSettings, ILogger<EmailService> logger) : IEmailService
    {
        private MailConfiguration MailSettings { get; } = mailSettings.Value;
        private ILogger<EmailService> Logger { get; } = logger;

        public async Task SendAsync(MailRequest request)
        {
            try
            {
                // create message
                var email = new MimeMessage
                {
                    Sender = MailboxAddress.Parse(MailSettings.From)
                };
                email.From.Add(MailboxAddress.Parse(MailSettings.From));
                if (request.ListMailTo != null && request.ListMailTo.Count != 0)
                {
                    foreach (var item in request.ListMailTo)
                    {
                        email.To.Add(MailboxAddress.Parse(item));
                    }
                }
                else
                {
                    email.To.Add(MailboxAddress.Parse(request.To));
                }
                email.Subject = request.Subject;
                var builder = new BodyBuilder
                {
                    HtmlBody = request.Body
                };
                email.Body = builder.ToMessageBody();
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(MailSettings.Host, MailSettings.Port);
                await smtp.AuthenticateAsync(MailSettings.UserName, MailSettings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task SendMultiAsync(MailMultiRequest request)
        {
            try
            {
                // create message
                var email = new MimeMessage
                {
                    Sender = MailboxAddress.Parse(request.From)
                };
                foreach (var item in request.To)
                {
                    email.To.Add(MailboxAddress.Parse(item));
                }
                email.Subject = request.Subject;
                var builder = new BodyBuilder
                {
                    HtmlBody = request.Body
                };
                email.Body = builder.ToMessageBody();
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(MailSettings.Host, MailSettings.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(MailSettings.UserName, MailSettings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task SendContactAsync(ContactRequest request)
        {
            try
            {
                // create message
                var email = new MimeMessage
                {
                    Sender = MailboxAddress.Parse(MailSettings.From)
                };
                email.From.Add(MailboxAddress.Parse(MailSettings.From));
                email.To.Add(MailboxAddress.Parse(MailSettings.From));
                email.Subject = "Contact Us";
                var body = "<p>-Your name:<br/>" + request.Name +
                           "<br/>-Email address:<br/>" + request.Email +
                           "<br/>-What services we help?:<br/>" + request.Service +
                           "<br/>-Company name:<br/>" + request.Company +
                           "<br/>-Tell us a little about what you're looking for?:<br/>" + request.Description + "</p>";
                var builder = new BodyBuilder
                {
                    HtmlBody = body
                };
                email.Body = builder.ToMessageBody();
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(MailSettings.Host, MailSettings.Port);
                await smtp.AuthenticateAsync(MailSettings.UserName, MailSettings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, ex);
            }
        }
    }
}