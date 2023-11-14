using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RevengineEditor.Utilities.Controls
{
    internal class ScalarBox : NumberBox
    {
        static ScalarBox()
        {
            // Override styles for WPF to allow WPF to customize it
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScalarBox),
                new FrameworkPropertyMetadata(typeof(ScalarBox)));
        }
    }
}
