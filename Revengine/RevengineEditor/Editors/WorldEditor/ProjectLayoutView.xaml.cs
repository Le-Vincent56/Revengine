using RevengineEditor.Components;
using RevengineEditor.GameProject;
using RevengineEditor.Utilities;
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

namespace RevengineEditor.Editors
{
    /// <summary>
    /// Interaction logic for ProjectLayoutView.xaml
    /// </summary>
    public partial class ProjectLayoutView : UserControl
    {
        public ProjectLayoutView()
        {
            InitializeComponent();
        }

        private void OnAddGrievance_Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Scene viewModel = button.DataContext as Scene;
            viewModel.AddGrievanceCommand.Execute(new Grievance(viewModel) { Name = "Empty Grievance"});
        }

        private void OnGrievances_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Retrieve the listBox
            ListBox listBox = sender as ListBox;

            // Cast the new selection to a list of Grievances
            List<Grievance> newSelection = listBox.SelectedItems.Cast<Grievance>().ToList();
            List<Grievance> previousSelection = newSelection.Except(e.AddedItems.Cast<Grievance>()).Concat(e.RemovedItems.Cast<Grievance>()).ToList();

            // Assign Undo/Redo actions
            Project.UndoRedo.Add(new UndoRedoAction(
                () => 
                {
                    listBox.UnselectAll();
                    previousSelection.ForEach(x => (listBox.ItemContainerGenerator.ContainerFromItem(x) as ListBoxItem).IsSelected = true);
                },
                () => 
                {
                    listBox.UnselectAll();
                    newSelection.ForEach(x => (listBox.ItemContainerGenerator.ContainerFromItem(x) as ListBoxItem).IsSelected = true);
                },
                "Selection changed"
                ));

            MSGrievance msGrievance = null;
            if(newSelection.Any())
            {
                msGrievance = new MSGrievance(newSelection);
            }
            GrievancesView.Instance.DataContext = msGrievance;
        }
    }
}
