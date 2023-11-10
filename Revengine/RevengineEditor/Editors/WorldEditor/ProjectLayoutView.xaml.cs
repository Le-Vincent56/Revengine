using RevengineEditor.Components;
using RevengineEditor.GameProject;
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
            GameEntity entity = (sender as ListBox).SelectedItems[0] as GameEntity;
            GameEntityView.Instance.DataContext = entity;
        }
    }
}
