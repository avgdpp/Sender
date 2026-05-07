using Sender.Services.Logger;
using System.Text;
using System.Text.RegularExpressions;

namespace Sender.Services.Notifications
{
    internal class PushNotificationService : INotificationService
    {
        private readonly ILogger _logger;

        public PushNotificationService(ILogger logger)
        {
            _logger = logger;
        }

        public string Name => "Push";

        public void Send(string message, string addressee)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Сообщение не может быть пустым");

            if (!IsTopicValid(addressee))
                throw new ArgumentException("Неправильно указан topic для push");

            string topic = addressee.Trim();
            string url = $"https://ntfy.sh/{topic}";

            using HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);

            using StringContent content = new StringContent(message, Encoding.UTF8, "text/plain");

            try
            {
                HttpResponseMessage response = client.PostAsync(url, content).GetAwaiter().GetResult();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Ошибка отправки push. Код: {(int)response.StatusCode}");
                }

                _logger.Info($"Push отправлен в topic {topic}. Сообщение: {message}");
            }
            catch (TaskCanceledException)
            {
                throw new Exception("Не удалось подключиться к сервису push");
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Ошибка подключения к сервису push: {ex.Message}");
            }
        }

        private bool IsTopicValid(string topic)
        {
            if (string.IsNullOrWhiteSpace(topic))
                return false;

            return Regex.IsMatch(topic.Trim(), @"^[a-zA-Z0-9_-]{3,64}$");
        }
    }
}
