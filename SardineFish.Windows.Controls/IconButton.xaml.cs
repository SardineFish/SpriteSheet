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
    /// IconButton.xaml 的交互逻辑
    /// </summary>
    public partial class IconButton : UserControl
    {
        public IconButton()
        {
            InitializeComponent();
            ContentBrush = NormalBrush;
        }

        public static DependencyProperty MouseDownBrushProperty = DependencyProperty.Register("MouseDownBrush", typeof(Brush), typeof(ButtonRadiu));
        public Brush MouseDownBrush
        {
            get
            {
                return (Brush)GetValue(MouseDownBrushProperty);
            }
            set
            {
                SetValue(MouseDownBrushProperty, value);
            }
        }

        public static DependencyProperty NormalBrushProperty = DependencyProperty.Register("NormalBrush", typeof(Brush), typeof(ButtonRadiu));
        public Brush NormalBrush
        {
            get
            {
                return (Brush)GetValue(NormalBrushProperty);
            }
            set
            {
                SetValue(NormalBrushProperty, value);
            }
        }

        public static DependencyProperty MouseEnterBrushProperty = DependencyProperty.Register("MouseEnterBrush", typeof(Brush), typeof(ButtonRadiu));
        public Brush MouseEnterBrush
        {
            get
            {
                return (Brush)GetValue(MouseEnterBrushProperty);
            }
            set
            {
                SetValue(MouseEnterBrushProperty, value);
            }
        }

        public static DependencyProperty ContentBrushProperty = DependencyProperty.Register("ContentBrush", typeof(Brush), typeof(ButtonRadiu));
        public Brush ContentBrush
        {
            get
            {
                return (Brush)GetValue(ContentBrushProperty);
            }
            internal set
            {
                SetValue(ContentBrushProperty, value);
            }
        }

        private void iconButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            ContentBrush = MouseDownBrush;
        }

        private void iconButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ContentBrush = MouseEnterBrush;
        }

        private void iconButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ContentBrush = NormalBrush;
        }

        private void iconButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Click != null)
            {
                Click.Invoke(this, e);
            }
        }

        public event EventHandler Click;
    }
}
