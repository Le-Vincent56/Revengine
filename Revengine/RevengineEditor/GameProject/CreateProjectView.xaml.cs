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
    /// Interaction logic for CreateProjectView.xaml
    /// </summary>
    public partial class CreateProjectView : UserControl
    {
        public CreateProjectView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Create a New Project
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCreate_Button_Click(object sender, RoutedEventArgs e)
        {
            // Initialize the ViewModel
            CreateProject viewModel = DataContext as CreateProject;

            // Create a new project using the selected template as the project template
            string projectPath = viewModel.CreateNewProject(templateListBox.SelectedItem as ProjectTemplate);

            // Check the dialog result
            bool dialogResult = false;
            if(!string.IsNullOrEmpty(projectPath))
            {
                // IF the string is valid, then the dialog result is true
                dialogResult = true;
            }

            // SEt the dialog result to this Window and close it
            Window window = Window.GetWindow(this);
            window.DialogResult = dialogResult;
            window.Close();
        }
    }
}
