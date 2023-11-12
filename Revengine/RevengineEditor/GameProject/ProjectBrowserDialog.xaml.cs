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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RevengineEditor.GameProject
{
    /// <summary>
    /// Interaction logic for ProjectBrowserDialg.xaml
    /// </summary>
    public partial class ProjectBrowserDialog : Window
    {
        private readonly CubicEase _easing = new CubicEase() { EasingMode = EasingMode.EaseInOut };
        public ProjectBrowserDialog()
        {
            InitializeComponent();
            Loaded += OnProjectBrowserDialogLoaded;
        }
        private void OnProjectBrowserDialogLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnProjectBrowserDialogLoaded;

            // Show the Create project tab when there are no projects to load
            if(!OpenProject.Projects.Any())
            {
                openProjectButton.IsEnabled = false;
                openProjectView.Visibility = Visibility.Hidden;
                OnToggleButton_Click(createProjectButton, new RoutedEventArgs());
            }
        }

        private void AnimateToCreateProject()
        {
            // Create a new DoubleAnimation
            DoubleAnimation highlightAnimation = new DoubleAnimation(125, 400, new Duration(TimeSpan.FromSeconds(0.2)));

            // Set Easing function
            highlightAnimation.EasingFunction = _easing;

            // Assign a Completed event
            highlightAnimation.Completed += (s, e) =>
            {
                ThicknessAnimation animation = new ThicknessAnimation(new Thickness(0), new Thickness(-1600, 0, 0, 0), new Duration(TimeSpan.FromSeconds(0.5)));
                animation.EasingFunction = _easing;
                browserContent.BeginAnimation(MarginProperty, animation);
            };

            // Begin the animation
            highlightRect.BeginAnimation(Canvas.LeftProperty, highlightAnimation);
        }

        private void AnimateToOpenProject()
        {
            // Create a new DoubleAnimation
            DoubleAnimation highlightAnimation = new DoubleAnimation(400, 125, new Duration(TimeSpan.FromSeconds(0.2)));

            // Set Easing function
            highlightAnimation.EasingFunction = _easing;

            // Assign a Completed event
            highlightAnimation.Completed += (s, e) =>
            {
                ThicknessAnimation animation = new ThicknessAnimation(new Thickness(-1600, 0, 0, 0), new Thickness(0), new Duration(TimeSpan.FromSeconds(0.5)));
                animation.EasingFunction = _easing;
                browserContent.BeginAnimation(MarginProperty, animation);
            };

            // Begin the animation
            highlightRect.BeginAnimation(Canvas.LeftProperty, highlightAnimation);
        }
        private void OnToggleButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if already in the "Open Project" window
            if(sender == openProjectButton)
            {
                // Check if coming from the "Create Project" window
                if(createProjectButton.IsChecked == true)
                {
                    createProjectButton.IsChecked = false;
                    AnimateToOpenProject();
                    openProjectView.IsEnabled = true;
                    createProjectView.IsEnabled = false;
                }
                openProjectButton.IsChecked = true;
            } else
            {
                if(openProjectButton.IsChecked == true)
                {
                    openProjectButton.IsChecked = false;
                    AnimateToCreateProject();
                    openProjectView.IsEnabled = false;
                    createProjectView.IsEnabled = true;
                }
                createProjectButton.IsChecked = true;
            }
        }
    }
}
