using Sender.Services.Logger;
using System.Net;
using System.Net.Mail;

namespace Sender.Services.Notifications
{
    internal class EmailService : INotificationService
    {
        private readonly ILogger _logger;

        public EmailService(ILogger logger)
        {
            _logger = logger;
        }

        public string Name => "Email";

        public void Send(string message, string addressee)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Сообщение не может быть пустым");

            if (!IsEmailValid(addressee))
                throw new ArgumentException("Неправильно набрана почта");

            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;

            string smtpUsername = "kirillafanasev7257@gmail.com";
            string smtpPassword = "";
            //omhh lotb tbhp vecu

            if (string.IsNullOrWhiteSpace(smtpUsername) || string.IsNullOrWhiteSpace(smtpPassword))
                throw new InvalidOperationException("Не заданы SENDER_EMAIL или SENDER_EMAIL_PASSWORD");

            using SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort);
            smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
            smtpClient.EnableSsl = true;
            smtpClient.Timeout = 10000;

            using MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(smtpUsername);
            mailMessage.To.Add(addressee);
            mailMessage.Subject = "Уведомление";
            mailMessage.Body = message;

            smtpClient.Send(mailMessage);
            _logger.Info($"Email отправлен получателю {addressee}. Сообщение: {message}");
        }

        private bool IsEmailValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                MailAddress mailAddress = new MailAddress(email);
                return mailAddress.Address.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}
