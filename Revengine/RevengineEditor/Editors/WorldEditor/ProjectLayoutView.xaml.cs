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

        private void OnAddGameEntity_Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Scene viewModel = button.DataContext as Scene;
            viewModel.AddGameEntityCommand.Execute(new GameEntity(viewModel) { Name = "Empty Game Entity"});
        }

        private void OnGameEntities_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GameEntityView.Instance.DataContext = null;

            // Retrieve the listBox
            ListBox listBox = sender as ListBox;

            // Check if any new items have been added to the selection
            if (e.AddedItems.Count > 0)
            {
                // If there are new items, set the data context to the first item
                GameEntityView.Instance.DataContext = listBox.SelectedItems[0];
            }

            // Cast the new selection to a list of GameEntities
            List<GameEntity> newSelection = listBox.SelectedItems.Cast<GameEntity>().ToList();
            List<GameEntity> previousSelection = newSelection.Except(e.AddedItems.Cast<GameEntity>()).Concat(e.RemovedItems.Cast<GameEntity>()).ToList();

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

        }
    }
}
