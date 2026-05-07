namespace Sender.Services.Notifications
{
    internal class NotificationSender
    {
        private readonly INotificationService _notificationService;

        public NotificationSender(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public void Send(string message, string addressee)
        {
            _notificationService.Send(message, addressee);
        }
    }
}
