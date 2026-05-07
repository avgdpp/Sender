using Sender.Services.Logger;
using Sender.Services.Notifications;
using System.Text.RegularExpressions;

namespace Sender
{
    public class MainForm : Form
    {
        private readonly IReadOnlyList<INotificationService> _services;
        private readonly ILogger _logger;
        private ComboBox _sendTypeComboBox = null!;
        private TextBox _messageTextBox = null!;
        private TextBox _addresseeTextBox = null!;
        private Button _sendMessageButton = null!;
        private RichTextBox _logRichTextBox = null!;

        internal MainForm(IReadOnlyList<INotificationService> services, ILogger logger)
        {
            _services = services;
            _logger = logger;
            Build();
        }

        private void Build()
        {
            Text = "Отправка уведомлений";
            Width = 650;
            Height = 520;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            _sendTypeComboBox = new ComboBox();
            _sendTypeComboBox.Left = 100;
            _sendTypeComboBox.Top = 20;
            _sendTypeComboBox.Width = 180;
            _sendTypeComboBox.Height = 25;
            _sendTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _sendTypeComboBox.Items.Add("Выберите тип");

            foreach (INotificationService service in _services)
            {
                _sendTypeComboBox.Items.Add(service.Name);
            }

            _sendTypeComboBox.SelectedIndex = 0;
            _sendTypeComboBox.SelectedIndexChanged += SendTypeChanged;
            Controls.Add(_sendTypeComboBox);

            _messageTextBox = new TextBox();
            _messageTextBox.Left = 100;
            _messageTextBox.Top = 100;
            _messageTextBox.Width = 400;
            _messageTextBox.Height = 25;
            _messageTextBox.PlaceholderText = "Введите сообщение";
            Controls.Add(_messageTextBox);

            _addresseeTextBox = new TextBox();
            _addresseeTextBox.Left = 100;
            _addresseeTextBox.Top = 150;
            _addresseeTextBox.Width = 400;
            _addresseeTextBox.Height = 25;
            _addresseeTextBox.PlaceholderText = "Сначала выберите способ отправки";
            Controls.Add(_addresseeTextBox);

            _sendMessageButton = new Button();
            _sendMessageButton.Top = 190;
            _sendMessageButton.Left = 100;
            _sendMessageButton.Width = 180;
            _sendMessageButton.Height = 35;
            _sendMessageButton.Text = "Отправить уведомление";
            _sendMessageButton.Click += SendButtonClick;
            Controls.Add(_sendMessageButton);

            _logRichTextBox = new RichTextBox();
            _logRichTextBox.Left = 100;
            _logRichTextBox.Top = 250;
            _logRichTextBox.Width = 400;
            _logRichTextBox.Height = 180;
            _logRichTextBox.ReadOnly = true;
            Controls.Add(_logRichTextBox);
        }

        private void SendTypeChanged(object? sender, EventArgs e)
        {
            string selectedType = _sendTypeComboBox.SelectedItem?.ToString() ?? string.Empty;

            switch (selectedType)
            {
                case "Email":
                    _addresseeTextBox.PlaceholderText = "Email получателя";
                    break;
                case "Push":
                    _addresseeTextBox.PlaceholderText = "Topic получателя";
                    break;
                case "SMS":
                    _addresseeTextBox.PlaceholderText = "Номер получателя";
                    break;
                default:
                    _addresseeTextBox.PlaceholderText = "Сначала выберите способ отправки";
                    break;
            }

            _messageTextBox.Text = string.Empty;
            _addresseeTextBox.Text = string.Empty;

            if (selectedType != "Выберите тип")
            {
                string log = $"Выбран сервис: {selectedType}";
                _logger.Info(log);
                AddLog(log);
            }
        }

        private void SendButtonClick(object? sender, EventArgs e)
        {
            string message = _messageTextBox.Text.Trim();
            string addressee = _addresseeTextBox.Text.Trim();

            if (_sendTypeComboBox.SelectedIndex == 0)
            {
                ShowError("Сначала выберите способ отправки");
                return;
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                ShowError("Введите сообщение");
                return;
            }

            if (string.IsNullOrWhiteSpace(addressee))
            {
                ShowError("Введите получателя");
                return;
            }

            INotificationService service = _services.First(service => service.Name == _sendTypeComboBox.SelectedItem!.ToString());
            string messageToSend = ReplaceMessageIfContainsCat(message);
            NotificationSender notificationSender = new NotificationSender(service);

            try
            {
                string startLog = $"Отправка. Сервис: {service.Name}. Получатель: {addressee}. Сообщение: {messageToSend}";
                _logger.Info(startLog);
                AddLog(startLog);
                notificationSender.Send(messageToSend, addressee);
                string successLog = $"Уведомление отправлено. Сервис: {service.Name}. Получатель: {addressee}";
                _logger.Info(successLog);
                AddLog(successLog);
                MessageBox.Show("Уведомление отправлено", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private string ReplaceMessageIfContainsCat(string message)
        {
            if (Regex.IsMatch(message, @"(^|\P{L})кот(\P{L}|$)", RegexOptions.IgnoreCase))
            {
                string link = "https://youtu.be/9FjGP4t2zKY?si=d5fX30bM4rcxVJ68";
                string log = "Пасхалка активирована: получите кота";
                _logger.Info(log);
                AddLog(log);
                return link;
            }

            return message;
        }

        private void ShowError(string text)
        {
            _logger.Error(text);
            AddLog($"Ошибка: {text}");
            MessageBox.Show(text, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void AddLog(string text)
        {
            _logRichTextBox.AppendText($"{DateTime.Now:dd.MM.yyyy HH:mm:ss} | {text}{Environment.NewLine}");
        }
    }
}
