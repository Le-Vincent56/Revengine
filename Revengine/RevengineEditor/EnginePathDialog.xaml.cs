using System;
using System.Collections.Generic;
using System.IO;
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

namespace RevengineEditor
{
    /// <summary>
    /// Interaction logic for EnginePathDialog.xaml
    /// </summary>
    public partial class EnginePathDialog : Window
    {
        public string RevenginePath { get; private set; }
        public EnginePathDialog()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
        }

        private void OnOk_Button_Click(object sender, RoutedEventArgs e)
        {
            // Get the text in the textBox
            string path = pathTextBox.Text;

            // Set the textblock to empty
            messageTextBlock.Text = string.Empty;

            // Test the text to see if it is bad or invalid
            if (string.IsNullOrEmpty(path))
            {
                messageTextBlock.Text = "Invalid Path";
            }
            else if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                messageTextBlock.Text = "Invalid Character(s)";
            }
            else if (!Directory.Exists(Path.Combine(path, @"Engine\EngineAPI\")))
            {
                messageTextBlock.Text = "Unable to Find Revengine at the Specified Location";
            }

            // If there's nothing in the messageTextBlock, that means the path is valid
            if (string.IsNullOrEmpty(messageTextBlock.Text))
            {
                // Add a separator at the end of the path if it doesn't have one already
                if (!Path.EndsInDirectorySeparator(path))
                    path += @"\";

                // Set the path
                RevenginePath = path;

                // Resolve and close the dialog
                DialogResult = true;
                Close();
            }
        }
    }
}
