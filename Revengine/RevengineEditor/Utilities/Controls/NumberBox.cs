using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace RevengineEditor.Utilities.Controls
{
    [TemplatePart(Name = "PART_textBlock", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_textBox", Type = typeof(TextBox))]
    public class NumberBox : Control
    {
        private double _originalValue;
        private double _mouseXStart;
        private double _multiplier;
        private bool _captured = false;
        private bool _valueChanged = false;

        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public double Multiplier
        {
            get => (double)GetValue(MultiplierProperty);
            set => SetValue(MultiplierProperty, value);
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(string), typeof(NumberBox),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty MultiplierProperty =
            DependencyProperty.Register(nameof(Multiplier), typeof(double), typeof(NumberBox),
            new PropertyMetadata(1.0));

        static NumberBox()
        {
            // Override styles for WPF to allow WPF to customize it
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumberBox),
                new FrameworkPropertyMetadata(typeof(NumberBox)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("PART_textBlock") is TextBlock textBlock)
            {
                textBlock.MouseLeftButtonDown += OnTextBlock_Mouse_LBD;
                textBlock.MouseLeftButtonUp += OnTextBlock_Mouse_LBU;
                textBlock.MouseMove += OnTextBlock_Mouse_Move;
            }
        }

        private void OnTextBlock_Mouse_Move(object sender, MouseEventArgs e)
        {
            if (_captured)
            {
                // Get the current mouse position with respect to the control
                double mouseX = e.GetPosition(this).X;

                // Get the distance from the start to the current position
                double distance = mouseX - _mouseXStart;

                // Check that the distance is greater than the minimum needed to "move"
                if (Math.Abs(distance) > SystemParameters.MinimumHorizontalDragDistance)
                {
                    // If Ctrl is held down, have more control over the drag
                    if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                    {
                        _multiplier = 0.001;
                    }
                    else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                    {
                        // If Shift is held down, have a greater increase  
                        _multiplier = 0.1;
                    }
                    else _multiplier = 0.01; // Default multiplier

                    // Add the distance to the original value
                    double newValue = _originalValue + (distance * _multiplier * Multiplier);

                    // Round to 5 decimal places
                    Value = newValue.ToString("0.##");

                    // Set value changed
                    _valueChanged = true;
                }
            }
        }

        private void OnTextBlock_Mouse_LBU(object sender, MouseButtonEventArgs e)
        {
            if (_captured)
            {
                // Reset captured
                Mouse.Capture(null);
                _captured = false;
                e.Handled = true;

                if (!_valueChanged && GetTemplateChild("PART_textBox") is TextBox textBox)
                {
                    // Select everything in the textBox
                    textBox.Visibility = Visibility.Visible;
                    textBox.Focus();
                    textBox.SelectAll();
                }
            }
        }

        private void OnTextBlock_Mouse_LBD(object sender, MouseButtonEventArgs e)
        {
            // Attempt to set the original data
            double.TryParse(Value, out _originalValue);

            // Capture the mouse
            Mouse.Capture(sender as UIElement);
            _captured = true;
            _valueChanged = false;

            // Prevent any further mouse event action
            e.Handled = true;

            // Get the position with respect to this control
            _mouseXStart = e.GetPosition(this).X;

            // Give the NumberBox the focus
            Focus();
        }
    }
}
