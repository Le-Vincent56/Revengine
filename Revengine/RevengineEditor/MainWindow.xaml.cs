using RevengineEditor.GameProject;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;

namespace RevengineEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string RevenginePath { get; private set; } = @"C:\Users\levin\source\repos\Revengine\Revengine";

        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnMainWindowLoaded;
            Closing += OnMainWindowClosed;
        }

        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnMainWindowLoaded;
            GetEnginePath();
            OpenProjectBrowserDialog();
        }

        private void OnMainWindowClosed(object sender, CancelEventArgs e)
        {
            Closing -= OnMainWindowClosed;

            // Unload a project if necessary
            Project.Current?.UnloadProject();
        }

        /// <summary>
        /// Get the local path of the Engine
        /// </summary>
        private void GetEnginePath()
        {
            // Check if the Environment variable is set
            string? revenginePath = Environment.GetEnvironmentVariable("REVENGINE_ENGINE", EnvironmentVariableTarget.User);
            if (revenginePath == null || !Directory.Exists(Path.Combine(revenginePath, @"Engine\EngineAPI")))
            {
                EnginePathDialog dialog = new EnginePathDialog();
                if(dialog.ShowDialog() == true)
                {
                    // Set the path and environmen variables
                    RevenginePath = dialog.RevenginePath;
                    Environment.SetEnvironmentVariable("REVENGINE_ENGINE", RevenginePath.ToUpper(), EnvironmentVariableTarget.User);
                } else
                {
                    // Shutdown
                    Application.Current.Shutdown();
                }
            } else
            {
                // Set the path
                RevenginePath = revenginePath;
            }
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
