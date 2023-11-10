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
    /// Interaction logic for GrievancesView.xaml
    /// </summary>
    public partial class GrievancesView : UserControl
    {
        public static GrievancesView Instance { get; private set; }
        private Action _undoAction;
        private string _propertyName;
        public GrievancesView()
        {
            InitializeComponent();
            DataContext = null;
            Instance = this;

            DataContextChanged += (_, __) =>
            {
                if (DataContext != null)
                {
                    (DataContext as MSObject).PropertyChanged += (s, e) => _propertyName = e.PropertyName;
                }
            };
        }

        private Action GetRenameAction()
        {
            // Retrieve DataContext
            MSObject viewModel = DataContext as MSObject;

            // Get the names before changes
            var selection = viewModel.SelectedGrievances.Select(grievance => (grievance, grievance.Name)).ToList();

            return new Action(() =>
            {
                // Restore old names
                selection.ForEach(item => item.grievance.Name = item.Name);

                // Refresh the DataContext
                (DataContext as MSObject).Refresh();
            });
        }

        private Action GetIsEnabledAction()
        {
            // Retrieve DataContext
            MSObject viewModel = DataContext as MSObject;

            // Get the names before changes
            var selection = viewModel.SelectedGrievances.Select(grievance => (grievance, grievance.IsEnabled)).ToList();

            return new Action(() =>
            {
                // Restore old names
                selection.ForEach(item => item.grievance.IsEnabled = item.IsEnabled);

                // Refresh the DataContext
                (DataContext as MSObject).Refresh();
            });
        }

        private void OneName_TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // Get the rename action
            _undoAction = GetRenameAction();
        }

        private void OneName_TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if(_propertyName == nameof(MSObject.Name) && _undoAction != null)
            {
                // Get the rename action
                Action redoAction = GetRenameAction();

                // Add the undo redo action
                Project.UndoRedo.Add(new UndoRedoAction(_undoAction, redoAction, "Renamed Grievances"));

                // Set property name to null
                _propertyName = null;
            }

            // Set undo action to null
            _undoAction = null;
        }

        private void OnIsEnabled_CheckBox_Click(object sender, RoutedEventArgs e)
        {
            // Get an undo action
            Action undoAction = GetIsEnabledAction();

            // Get the view model and set a current IsEnabled
            MSObject viewModel = DataContext as MSObject;
            viewModel.IsEnabled = (sender as CheckBox).IsChecked == true;

            // Get a redo action
            Action redoAction = GetIsEnabledAction();

            // Add the UndoRedo actions
            Project.UndoRedo.Add(new UndoRedoAction(undoAction, redoAction, viewModel.IsEnabled == true ? "Enabled Grievances" : "Disabled Grievances"));
        }
    }
}
