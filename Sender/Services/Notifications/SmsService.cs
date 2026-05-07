using Sender.Services.Logger;
using System.Net;
using System.Text.RegularExpressions;

namespace Sender.Services.Notifications
{
    internal class SmsService : INotificationService
    {
        private readonly ILogger _logger;

        public SmsService(ILogger logger)
        {
            _logger = logger;
        }

        public string Name => "SMS";

        public void Send(string message, string addressee)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Сообщение не может быть пустым");

            if (!IsPhoneValid(addressee))
                throw new ArgumentException("Неправильно набран номер");

            string phone = NormalizeNumber(addressee);
            string apiId = "CFD38630-3BA0-DA87-FD3D-BE1D592285CA";

            if (string.IsNullOrWhiteSpace(apiId))
                throw new InvalidOperationException("Не задан SMS_RU_API_ID");

            string encodedMessage = WebUtility.UrlEncode(message);
            string testValue = Environment.GetEnvironmentVariable("SMS_RU_TEST") ?? "1";
            string testPart = testValue == "1" ? "&test=1" : string.Empty;
            string url = $"https://sms.ru/sms/send?api_id={apiId}&to={phone}&msg={encodedMessage}&json=1{testPart}";

            using HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);

            try
            {
                string response = client.GetStringAsync(url).GetAwaiter().GetResult();
                _logger.Info($"SMS отправлено получателю {phone}. Ответ сервиса: {response}. Сообщение: {message}");
            }
            catch (TaskCanceledException)
            {
                throw new Exception("Не удалось подключиться к SMS.ru");
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Ошибка подключения к SMS.ru: {ex.Message}");
            }
        }

        private bool IsPhoneValid(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            string cleaned = NormalizeNumber(phone);
            return Regex.IsMatch(cleaned, @"^79\d{9}$");
        }

        private string NormalizeNumber(string phone)
        {
            string cleaned = Regex.Replace(phone, @"[\s\-\(\)]", string.Empty);

            if (cleaned.StartsWith("+7"))
            {
                cleaned = "7" + cleaned.Substring(2);
            }
            else if (cleaned.StartsWith("8"))
            {
                cleaned = "7" + cleaned.Substring(1);
            }
            else if (cleaned.Length == 10 && cleaned.StartsWith("9"))
            {
                cleaned = "7" + cleaned;
            }

            return cleaned;
        }
    }
}
