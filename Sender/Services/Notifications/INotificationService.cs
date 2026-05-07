namespace Sender.Services.Notifications
{
    internal interface INotificationService
    {
        string Name { get; }
        void Send(string message, string addressee);
    }
}
