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
using System.Windows.Media.Animation;

namespace SardineFish.Windows.Controls
{
    /// <summary>
    /// ScrollView.xaml 的交互逻辑
    /// </summary>
    public partial class ScrollView : UserControl
    {
        public ScrollView()
        {
            InitializeComponent();
            scrollContent.ScrollChanged += ScrollContent_ScrollChanged;
        }

        private void DrawScrollBar()
        {
            var top = (scrollContent.VerticalOffset / scrollContent.ExtentHeight) * scrollHeight.ActualHeight;
            top = (Double.IsNaN(top) ? 0 : top);
            DoubleAnimation animation = new DoubleAnimation(upArea.ActualHeight, top, new Duration(new TimeSpan(1)));
            upArea.BeginAnimation(Rectangle.HeightProperty, animation);
            if(scrollContent.ExtentHeight >scrollContent.ViewportHeight )
            {
                var viewHeight = (scrollContent.ViewportHeight / scrollContent.ExtentHeight) * scrollHeight.ActualHeight;
                viewHeight = (double.IsNaN(viewHeight) ? 0 : viewHeight);
                animation = new DoubleAnimation(scrollBlockV.ActualHeight, viewHeight, new Duration(new TimeSpan(1)));
                scrollBlockV.BeginAnimation(Rectangle.HeightProperty, animation);
            }
            else
            {
                animation = new DoubleAnimation(scrollBlockV.ActualHeight, scrollBlockV.ActualHeight, new Duration(new TimeSpan(1)));
                scrollBlockV.BeginAnimation(Rectangle.HeightProperty, animation);
            }
            
        }

        private void ScrollContent_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            SetValue(HorizontalOffsetProperty, e.HorizontalOffset);
            SetValue(VerticalOffsetProperty, e.VerticalOffset);
            SetValue(ViewportHeightProperty, e.ViewportHeight);
            SetValue(ViewportWidthProperty, e.ViewportWidth);
            DrawScrollBar();
            if (ScrollChanged != null)
                ScrollChanged.Invoke(this, e);
        }

        public static DependencyProperty HorizontalOffsetProperty = DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(ScrollView));

        public double HorizontalOffset
        {
            get
            {
                return (double)GetValue(HorizontalOffsetProperty);
            }
            set
            {
                scrollContent.ScrollToHorizontalOffset(value);
            }
        }

        public static DependencyProperty VerticalOffsetProperty = DependencyProperty.Register("VerticalOffset", typeof(double), typeof(ScrollView));
        public double VerticalOffset
        {
            get
            {
                return (double)GetValue(VerticalOffsetProperty);
            }
            set
            {
                scrollContent.ScrollToVerticalOffset(value);
            }
        }

        public static DependencyProperty ViewportHeightProperty = DependencyProperty.Register("ViewportHeight", typeof(double), typeof(ScrollView));
        public double ViewportHeight
        {
            get
            {
                return (double)GetValue(ViewportHeightProperty);
            }
        }

        public static DependencyProperty ViewportWidthProperty = DependencyProperty.Register("ViewportWidth", typeof(double), typeof(ScrollView));
        public double ViewportWidth
        {
            get
            {
                return (double)GetValue(ViewportWidthProperty);
            }
        }

        public static DependencyProperty ViewContentProperty = DependencyProperty.Register("ViewContent", typeof(object), typeof(ScrollView));
        public object ViewContent
        {
            get
            {
                return GetValue(ViewContentProperty);
            }
            set
            {
                SetValue(ViewContentProperty, value);
            }
        }

        event ScrollChangedEventHandler ScrollChanged;

        
    }
}
