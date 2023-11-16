using RevengineEditor.Components;
using RevengineEditor.GameProject;
using RevengineEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

namespace RevengineEditor.Editors
{
    /// <summary>
    /// Interaction logic for TransformView.xaml
    /// </summary>
    public partial class TransformView : UserControl
    {
        private Action _undoAction = null;
        private bool _propertyChanged = false;
        public TransformView()
        {
            InitializeComponent();
            Loaded += OnTransformViewLoaded;
        }

        private void OnTransformViewLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnTransformViewLoaded;

            // Whenever any property of the view model has changed, notify this class
            (DataContext as MSTransform).PropertyChanged += (s, e) => _propertyChanged = true;
        }

        private Action GetAction(Func<Transform, (Transform transform, Vector3)> selector,
            Action<(Transform transform, Vector3)> forEachAction)
        {
            // Check if the DataContext is correct
            if (!(DataContext is MSTransform vm))
            {
                _undoAction = null;
                _propertyChanged = false;
                return null;
            }

            // Get a list of the selected transforms
            var selection = vm.SelectedMotivators.Select(x => selector(x)).ToList();

            // Create the Undo Action
            return new Action(() =>
            {
                // Set old values
                selection.ForEach(x => forEachAction(x));

                // Refresh the transform component(s)
                (GrievancesView.Instance.DataContext as MSObject)?.GetMSComponent<MSTransform>().Refresh();
            });
        }

        private Action GetPositionAction() => GetAction((x) => (x, x.Position), (x) => x.transform.Position = x.Item2);
        private Action GetRotationAction() => GetAction((x) => (x, x.Rotation), (x) => x.transform.Rotation = x.Item2);
        private Action GetScaleAction() => GetAction((x) => (x, x.Scale), (x) => x.transform.Scale = x.Item2);

        private void RecordAction(Action redoAction, string name)
        {
            // Check if the property has been changed
            if (_propertyChanged)
            {
                Debug.Assert(_undoAction != null);

                // Reset propertyChanged
                _propertyChanged = false;

                // Add the UndoRedo Actiosn to the list
                Project.UndoRedo.Add(new UndoRedoAction(_undoAction, redoAction, name));
            }
        }

        private void OnPosition_VectorBox_PreviewMouse_LBD(object sender, MouseButtonEventArgs e)
        {
            // Reset propertyChanged
            _propertyChanged = false;

            // Get the Undo action
            _undoAction = GetPositionAction();
        }

        private void OnPosition_VectorBox_PreviewMouse_LBU(object sender, MouseButtonEventArgs e)
        {
            // Record the Action
            RecordAction(GetPositionAction(), "Position changed");
        }

        private void OnPosition_VectorBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (_propertyChanged && _undoAction != null)
            {
                OnPosition_VectorBox_PreviewMouse_LBU(sender, null);
            }
        }

        private void OnRotation_VectorBox_PreviewMouse_LBD(object sender, MouseButtonEventArgs e)
        {
            // Reset propertyChanged
            _propertyChanged = false;

            // Get the Undo action
            _undoAction = GetRotationAction();
        }

        private void OnRotation_VectorBox_PreviewMouse_LBU(object sender, MouseButtonEventArgs e)
        {
            // Record the Action
            RecordAction(GetRotationAction(), "Rotation changed");
        }

        private void OnRotation_VectorBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (_propertyChanged && _undoAction != null)
            {
                OnRotation_VectorBox_PreviewMouse_LBU(sender, null);
            }
        }

        private void OnScale_VectorBox_PreviewMouse_LBD(object sender, MouseButtonEventArgs e)
        {
            // Reset propertyChanged
            _propertyChanged = false;

            // Get the Undo action
            _undoAction = GetScaleAction();
        }

        private void OnScale_VectorBox_PreviewMouse_LBU(object sender, MouseButtonEventArgs e)
        {
            // Record the Action
            RecordAction(GetScaleAction(), "Scale changed");
        }

        private void OnScale_VectorBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (_propertyChanged && _undoAction != null)
            {
                OnScale_VectorBox_PreviewMouse_LBU(sender, null);
            }
        }
    }
}
