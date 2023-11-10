using RevengineEditor.GameProject;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace RevengineEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnMainWindowLoaded;
            Closing += OnMainWindowClosed;
        }

        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnMainWindowLoaded;
            OpenProjectBrowserDialog();
        }

        private void OnMainWindowClosed(object sender, CancelEventArgs e)
        {
            Closing -= OnMainWindowClosed;

            // Unload a project if necessary
            Project.Current?.UnloadProject();
        }

        /// <summary>
        /// Open the Project Browser Dialog
        /// </summary>
        private void OpenProjectBrowserDialog()
        {
            ProjectBrowserDialog projectBrowser = new ProjectBrowserDialog();

            // Check if the projectBrowser should be shown, or if the DataContext is null
            if (projectBrowser.ShowDialog() == false || projectBrowser.DataContext == null)
            {
                // If not, then shut down
                Application.Current.Shutdown();
            }
            else
            {
                // Unload a loaded project if it exists
                Project.Current?.UnloadProject();

                // Set the current DatContext to the projectBrowser DataContext
                DataContext = projectBrowser.DataContext;
            }
        }
    }
}
