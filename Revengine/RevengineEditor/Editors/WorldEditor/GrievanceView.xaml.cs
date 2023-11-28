using RevengineEditor.Components;
using RevengineEditor.GameProject;
using RevengineEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RevengineEditor.Editors
{
    public class NullableBoolToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool b && b == true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool b && b == true;
        }
    }

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
            // Set the property name to an empty string
            _propertyName = string.Empty;

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

        private void OnAddMotivator_Button_PreviewMouse_LBD(object sender, MouseButtonEventArgs e)
        {
            ContextMenu menu = FindResource("addMotivatorMenu") as ContextMenu;
            ToggleButton btn = sender as ToggleButton;
            btn.IsChecked = true;
            menu.Placement = PlacementMode.Bottom;
            menu.PlacementTarget = btn;
            menu.MinWidth = btn.ActualWidth;
            menu.IsOpen = true;
        }

        private void AddMotivator(MotivatorType motivatorType, object data)
        {
            Func<Grievance, object, Motivator> creationFunction = MotivatorFactory.GetCreationFunction(motivatorType);
            List<(Grievance grievance, Motivator motivator)> changedEntities = new List<(Grievance grievance, Motivator motivator)>();
            MSObject vm = DataContext as MSObject;

            // Look through all the selected objects
            foreach(Grievance grievance in vm.SelectedGrievances)
            {
                // Get the Motivator
                Motivator motivator = creationFunction(grievance, data);

                // Try to add the Motivator to each Grievance
                if(grievance.AddMotivator(motivator))
                {
                    changedEntities.Add((grievance, motivator));
                }
            }

            // If there are any changed entities, add actions to the UndoRedo manager
            if(changedEntities.Any())
            {
                vm.Refresh();

                Project.UndoRedo.Add(new UndoRedoAction(
                    () =>
                    {
                        // Remove the newly added Motivator
                        changedEntities.ForEach(x => x.grievance.RemoveMotivator(x.motivator));
                        (DataContext as MSObject).Refresh();
                    },
                    () =>
                    {
                        // Redo - add the Motivator back
                        changedEntities.ForEach(x => x.grievance.AddMotivator(x.motivator));
                        (DataContext as MSObject).Refresh();
                    },
                    $"Added {motivatorType} motivator"
                ));
            }
        }

        private void OnAddScriptMotivator(object sender, RoutedEventArgs e)
        {
            AddMotivator(MotivatorType.Script, (sender as MenuItem).Header.ToString());
            MSObject vm = DataContext as MSObject;
        }
    }
}
