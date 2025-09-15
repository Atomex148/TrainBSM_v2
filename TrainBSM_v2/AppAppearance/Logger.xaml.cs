using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TrainBSM_v2.AppAppearance
{
    /// <summary>
    /// Логика взаимодействия для Logger.xaml
    /// </summary>
    /// 

    public enum LoggerMessageType
    {
        Normal,
        Warning,
        Error
    }

    public class LoggerMessage
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public required string Message { get; set; }
        public LoggerMessageType Type { get; set; } = LoggerMessageType.Normal;
        public string? MessageCode = null;

        public string DisplayText =>
                        MessageCode != null ? $"[{Timestamp:HH:mm:ss}]: {Message}; Код: {MessageCode}"
                        : $"[{Timestamp:HH:mm:ss}]: {Message}";
    }

    public partial class Logger : UserControl
    {
        public ObservableCollection<LoggerMessage> Logs { get; } = new();
        public Brush LoggerBackground => MainGrid.Background;

        public Logger()
        {
            InitializeComponent();
            LogItems.ItemsSource = Logs;
        }

        public void AddLog(string message, LoggerMessageType type = LoggerMessageType.Normal)
        {
            Logs.Add(new LoggerMessage { Message = message, Type = type });
        }

        public void AddLog(DieselMessage dieselMessage) { 
            Logs.Add(new LoggerMessage { Message = dieselMessage.Message, 
                Type = dieselMessage.DisplayType, MessageCode = dieselMessage.Code });
        }

        public void DeleteLastLog()
        {
            if (Logs.Any())
                Logs.RemoveAt(Logs.Count - 1);
        }

        public void DeleteLogAt(int index)
        {
            if (index < 0) return;
            if (index >= Logs.Count) DeleteLastLog();
            else Logs.RemoveAt(index);
        }
        private void ClearLogButton_Click(object sender, EventArgs e)
        {
            Logs.Clear();
        }
    }
}
