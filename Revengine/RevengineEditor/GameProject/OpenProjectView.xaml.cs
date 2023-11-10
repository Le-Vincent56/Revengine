using System;
using System.Collections.Generic;
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

namespace RevengineEditor.GameProject
{
    /// <summary>
    /// Interaction logic for OpenProjectView.xaml
    /// </summary>
    public partial class OpenProjectView : UserControl
    {
        public OpenProjectView()
        {
            InitializeComponent();

            // Focus on the first selected index
            Loaded += (s, e) =>
            {
                ListBoxItem item = projectsListBox.ItemContainerGenerator.ContainerFromIndex(projectsListBox.SelectedIndex) as ListBoxItem;
                item?.Focus();
            };
        }

        /// <summary>
        /// Open a selected project by double-clicking it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnListBoxItem_Mouse_DoubleClick(object sender, RoutedEventArgs e)
        {
            OpenSelectedProject();
        }

        /// <summary>
        /// Open a selected project through the Open button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOpen_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenSelectedProject();
        }

        /// <summary>
        /// Open a selected project
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenSelectedProject()
        {
            // Find the project at the selected item
            Project project = OpenProject.Open(projectsListBox.SelectedItem as ProjectData);

            // Set the dialog result to this Window and close it
            Window window = Window.GetWindow(this);

            // Check the dialog result
            bool dialogResult = false;
            if (project != null)
            {
                // If the project is not null, set the dialog result to be true
                dialogResult = true;

                // Set the DataContext to the project to be able to get it in the Main Window
                window.DataContext = project;
            }

            // Close the window
            window.DialogResult = dialogResult;
            window.Close();
        }
    }
}
