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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RevengineEditor.Editors
{
    [ContentProperty("MotivatorContent")]
    public partial class MotivatorView : UserControl
    {
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public FrameworkElement MotivatorContent
        {
            get { return (FrameworkElement)GetValue(MotivatorContentProperty); }
            set { SetValue(MotivatorContentProperty, value); }
        }

        // Dependency property as the backing store for Header - enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(string), typeof(MotivatorView));

        public static readonly DependencyProperty MotivatorContentProperty =
            DependencyProperty.Register(nameof(MotivatorContent), typeof(FrameworkElement), typeof(MotivatorView));

        public MotivatorView()
        {
            InitializeComponent();
        }
    }
}
