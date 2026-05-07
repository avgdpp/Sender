namespace Sender.Services.Logger
{
    internal class FileLogger : ILogger
    {
        private readonly string _filePath;

        public FileLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Info(string text)
        {
            Write("INFO", text);
        }

        public void Error(string text)
        {
            Write("ERROR", text);
        }

        private void Write(string level, string text)
        {
            string line = $"{DateTime.Now:dd.MM.yyyy HH:mm:ss} | {level} | {text}";
            File.AppendAllText(_filePath, line + Environment.NewLine);
        }
    }
}
