namespace Sender.Services.Logger
{
    internal interface ILogger
    {
        void Info(string text);
        void Error(string text);
    }
}
