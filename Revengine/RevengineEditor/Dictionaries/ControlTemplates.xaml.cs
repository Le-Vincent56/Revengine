using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace RevengineEditor.Dictionaries
{
    public partial class ControlTemplates : ResourceDictionary
    {
        private void OnTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            BindingExpression exp = textBox.GetBindingExpression(TextBox.TextProperty);

            // If there's no binding expression, return
            if (exp == null) return;

            if(e.Key == Key.Enter)
            {
                // Check if the textBox tag is correct and the command can be executed
                if (textBox.Tag is ICommand commmand && commmand.CanExecute(textBox.Text))
                {
                    // Execute the command
                    commmand.Execute(textBox.Text);
                } else
                {
                    // Otherwise, update the source
                    exp.UpdateSource();
                }

                // Clear the keyboard focus
                Keyboard.ClearFocus();

                // Prevent other keystrokes from being picked up by the textbox
                e.Handled = true;
            } else if(e.Key == Key.Escape)
            {
                // Read back the old value and clear focus
                exp.UpdateTarget();
                Keyboard.ClearFocus();
            }
        }

        private void OnTextBoxRename_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            BindingExpression exp = textBox.GetBindingExpression(TextBox.TextProperty);

            // If there's no binding expression, return
            if (exp == null) return;

            if (e.Key == Key.Enter)
            {
                // Check if the textBox tag is correct and the command can be executed
                if (textBox.Tag is ICommand commmand && commmand.CanExecute(textBox.Text))
                {
                    // Execute the command
                    commmand.Execute(textBox.Text);
                }
                else
                {
                    // Otherwise, update the source
                    exp.UpdateSource();
                }

                // Set the TextBox visibility
                textBox.Visibility = Visibility.Collapsed;

                // Prevent other keystrokes from being picked up by the textbox
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                // Read back the old value and clear focus
                exp.UpdateTarget();

                // Set the TextBox visibility
                textBox.Visibility = Visibility.Collapsed;
            }
        }

        private void OnTextBoxRename_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // If the textbox is not visible, do nothing
            if (!textBox.IsVisible) return;

            BindingExpression exp = textBox.GetBindingExpression(TextBox.TextProperty);

            if(exp != null)
            {
                // Update the binding and move the focus
                exp.UpdateTarget();

                // Collapse the textbox
                textBox.Visibility = Visibility.Collapsed;
            }
        }

        private void OnClose_Button_Click(object sender, RoutedEventArgs e)
        {
            // Close the window
            Window window = (Window)((FrameworkElement)sender).TemplatedParent;
            window.Close();
        }

        private void OnMaximizeRestore_Button_Click(object sender, RoutedEventArgs e)
        {
            Window window = (Window)((FrameworkElement)sender).TemplatedParent;
            window.WindowState = (window.WindowState == WindowState.Normal) ? WindowState.Maximized :
                WindowState.Normal;
        }

        private void OnMinimize_Button_Click(object sender, RoutedEventArgs e)
        {
            Window window = (Window)((FrameworkElement)sender).TemplatedParent;
            window.WindowState = WindowState.Minimized;
        }
    }
}
