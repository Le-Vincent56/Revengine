using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;

namespace RevengineEditor.Utilities
{
    public enum MessageType
    {
        Info = 0x01,
        Warning = 0x02,
        Error = 0x04
    }

    public class LogMessage
    {
        public DateTime Time { get; }
        public MessageType MessageType { get; }
        public string Message { get; }
        public string File { get; }
        public string Caller { get; }
        public int Line { get; }
        public string MetaData { get { return $"{File}: {Caller} ({Line})"; } }

        public LogMessage(MessageType type, string message, string file, string caller, int line)
        {
            Time = DateTime.Now;
            MessageType = type;
            Message = message;
            File = Path.GetFileName(file);
            Caller = caller;
            Line = line;
        }
    }

    public static class Logger
    {
        private static int _messageFilter = (int)(MessageType.Info | MessageType.Warning | MessageType.Error);
        private readonly static ObservableCollection<LogMessage> _messages = new ObservableCollection<LogMessage>();
        public static ReadOnlyObservableCollection<LogMessage> Messages { get; } = new ReadOnlyObservableCollection<LogMessage>(_messages);
        public static CollectionViewSource FilteredMessages { get; } = new CollectionViewSource() { Source = Messages };

        /// <summary>
        /// Log a message
        /// </summary>
        /// <param name="type">The MessageType</param>
        /// <param name="msg">The message to log</param>
        /// <param name="file">The file creating the message</param>
        /// <param name="caller">The caller of the message</param>
        /// <param name="line">The line causing the message</param>
        public static async void Log(MessageType type, string msg, [CallerFilePath]string file="", [CallerMemberName]string caller="", [CallerLineNumber]int line = 0)
        {
            // Wait for the correct thread to begin and invoke
            await Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                // Add the message to the collection of messages
                _messages.Add(new LogMessage(type, msg, file, caller, line));
            }));
        }

        /// <summary>
        /// Clear the messages collection
        /// </summary>
        public static async void Clear()
        {
            // Wait for the correct thread to begin and invoke
            await Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                // Clear the collection of messages
                _messages.Clear();
            }));
        }

        public static void SetMessageFilter(int mask)
        {
            // Set the filter
            _messageFilter = mask;

            // Refresh the filtered messages
            FilteredMessages.View.Refresh();
        }

        static Logger()
        {
            // Lambda expression for each filtered message
            FilteredMessages.Filter += (s, e) =>
            {
                // Get the bits of the MessageType
                int type = (int)(e.Item as LogMessage).MessageType;

                // See if the filtered bits contain the bits of the MessageType
                e.Accepted = (type & _messageFilter) != 0;
            };
        }
    }
}
