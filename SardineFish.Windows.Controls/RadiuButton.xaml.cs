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

namespace SardineFish.Windows.Controls
{
    /// <summary>
    /// ButtonRadiu.xaml 的交互逻辑
    /// </summary>
    public partial class ButtonRadiu : UserControl
    {
        public ButtonRadiu()
        {
            InitializeComponent();

        }

        public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ButtonRadiu));
        
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }

        }

        public event System.Windows.RoutedEventHandler Click;

        private void RadiuButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Click != null)
                Click(sender, new RoutedEventArgs());
        }

        private void RadiuButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}
