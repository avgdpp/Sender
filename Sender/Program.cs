using Sender.Services.IoC;
using Sender.Services.Logger;
using Sender.Services.Notifications;

namespace Sender
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            SimpleContainer container = new SimpleContainer();
            ILogger logger = new FileLogger("notification-log.txt");

            container.RegisterSingleton(logger);
            container.Register<INotificationService>(() => new EmailService(container.Resolve<ILogger>()));
            container.Register<INotificationService>(() => new PushNotificationService(container.Resolve<ILogger>()));
            container.Register<INotificationService>(() => new SmsService(container.Resolve<ILogger>()));
            container.Register(() => new MainForm(container.ResolveAll<INotificationService>(), container.Resolve<ILogger>()));

            Application.Run(container.Resolve<MainForm>());
        }
    }
}
