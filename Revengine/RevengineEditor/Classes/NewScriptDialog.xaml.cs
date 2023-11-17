using RevengineEditor.GameProject;
using RevengineEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace RevengineEditor.Classes
{
    /// <summary>
    /// Interaction logic for NewScriptDialog.xaml
    /// </summary>
    public partial class NewScriptDialog : Window
    {
        private static readonly string _cppCode = @"#include ""{0}.h""

namespace {1} {{
    REGISTER_SCRIPT({0});
    void {0}::begin_play() {{ 

    }}

    void {0}::update(float dt) {{ 

    }} 

    }} // namespace {1}";

        private static readonly string _hCode = @"#pragma once

    namespace {1} {{

	    class {0} : public revengine::script::grievance_script {{
	    public:
		    constexpr explicit {0}(revengine::grievance::grievance grievance)
			    : revengine::script::grievance_script{{grievance}} {{ }}
            void begin_play() override;
		    void update(float dt) override;
        private:
	    }};
    }} // namespace {1}";

        private static readonly string _namespace = GetNamespaceFromProjectName();

        public NewScriptDialog()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            scriptPath.Text = @"GameCode\";
        }

        private static string GetNamespaceFromProjectName()
        {
            // Replace all the spaces in a project name with underscores
            string projectName = Project.Current.Name;
            projectName = projectName.Replace(' ', '_');
            return projectName;
        }

        bool Validate()
        {
            bool isValid = false;
            string name = scriptName.Text.Trim();
            string path = scriptPath.Text.Trim();
            string errorMsg = string.Empty;

            // Check if the Script name or path are invalid
            if(string.IsNullOrEmpty(name))
            {
                errorMsg = "Empty Script Name";
            } else if(name.IndexOfAny(Path.GetInvalidFileNameChars()) != -1 || name.Any(x => char.IsWhiteSpace(x)))
            {
                errorMsg = "Invalid Character(s) used in Script Name";
            } else if(string.IsNullOrEmpty(path))
            {
                errorMsg = "Empty Script Path";
            } else if(path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                errorMsg = "Invalid Character(s) Used in Script Path";
            } else if(!Path.GetFullPath(Path.Combine(Project.Current.Path, path)).Contains(Path.Combine(Project.Current.Path, @"GameCode\")))
            {
                errorMsg = "Script Must Be Added to (A Sub-Folder Of) the GameCode folder";
            } else if(File.Exists(Path.GetFullPath(Path.Combine(Path.Combine(Project.Current.Path, path), $"{name}.cpp"))) ||
                File.Exists(Path.GetFullPath(Path.Combine(Path.Combine(Project.Current.Path, path), $"{name}.h"))))
            {
                errorMsg = $"Script {Name} Already Exists in This Folder";
            } else
            {
                isValid = true;
            }

            // Set colors for UI
            if(!isValid)
            {
                messageTextBlock.Foreground = FindResource("Editor.RedBrush") as Brush;
            } else
            {
                messageTextBlock.Foreground = FindResource("Editor.FontBrush") as Brush;
            }

            // Set the error message
            messageTextBlock.Text = errorMsg;

            return isValid;
        }

        private void OnScriptName_Textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Check if the script is valid
            if (!Validate())
                return;

            // Let the user know that the script will be added
            string name = scriptName.Text.Trim();
            messageTextBlock.Text = $"{name}.h and {name}.cpp wil be added to {Project.Current.Name}";
        }

        private void OnScriptPath_Textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Check if the path is valid
            Validate();
        }

        private async void OnCreate_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate())
                return;

            // Disable the window to allow the user to work
            IsEnabled = false;

            // Fade in the loading animation
            busyAnimation.Opacity = 0;
            busyAnimation.Visibility = Visibility.Visible;
            DoubleAnimation fadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(500)));
            busyAnimation.BeginAnimation(OpacityProperty, fadeIn);

            try
            {
                // Should capture the values of the variables locally or else we can't
                // use anything used by the UI thread
                string name = scriptName.Text.Trim();
                string path = Path.GetFullPath(Path.Combine(Project.Current.Path, scriptPath.Text.Trim()));
                string solution = Project.Current.Solution;
                string projectName = Project.Current.Name;

                await Task.Run(() => CreateScript(name, path, solution, projectName));
            } catch(Exception ex) {
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, $"Failed to create script {scriptName.Text}");
            } finally
            {
                // Fade out the loading animation and close the window
                DoubleAnimation fadeOut = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(200)));
                fadeOut.Completed += (s, e) =>
                {
                    busyAnimation.Opacity = 0;
                    busyAnimation.Visibility = Visibility.Hidden;
                    Close();
                };
                busyAnimation.BeginAnimation(OpacityProperty, fadeOut);
            }
        }

        private void CreateScript(string name, string path, string solution, string projectName)
        {
            // Check if the path exists - if not, create the path
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            // Get the path of the .cpp and .h files
            string cpp = Path.GetFullPath(Path.Combine(path, $"{name}.cpp"));
            string h = Path.GetFullPath(Path.Combine(path, $"{name}.h"));

            // Format the .cpp code
            using(StreamWriter sw = File.CreateText(cpp))
            {
                sw.Write(string.Format(_cppCode, name, _namespace));
            }

            // Format the .h code
            using(StreamWriter sw = File.CreateText(h))
            {
                sw.Write(string.Format(_hCode, name, _namespace));
            }

            // Store formatted string files into an array
            string[] files = new string[] { cpp, h };

            // Attempt to add the files to the solution 3 times
            for(int i = 0; i < 3; i++)
            {
                // Add the files to the Visual Studio solution
                if (!VisualStudio.AddFilesToSolution(solution, projectName, files))
                {
                    // Wait 1 second after each attempt
                    System.Threading.Thread.Sleep(1000);
                } else
                {
                    // Give up after 3 times
                    break;
                }
            }
        }
    }
}
