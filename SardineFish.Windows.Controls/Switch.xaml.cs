using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Shapes;
using System.ComponentModel;

namespace SardineFish.Windows.Controls
{
    public enum SwitchStatus
    {
        Off,
        On,
    }

    /// <summary>
    /// Switch.xaml 的交互逻辑
    /// </summary>
    public partial class Switch : UserControl
    {

        public class SwitchChangedEventArgs : EventArgs
        {
            public SwitchStatus SwitchStatus
            {
                get;
                private set;
            }
            public SwitchChangedEventArgs(SwitchStatus status)
            {
                SwitchStatus = status;
            }
        }

        
        public Switch()
        {
            
            InitializeComponent();
            /*if (this.SwitchStatus == SwitchStatus.On)
            {
                rectangle.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00B9FF"));
                switchButton.Fill = new SolidColorBrush(Colors.White);
                Storyboard sb = this.Resources["Storyboard1"] as Storyboard;
                sb.Stop();
                (sb.Children[0] as DoubleAnimationUsingKeyFrames).KeyFrames[0].Value = (this.ActualWidth - switchButton.ActualWidth - 2 * switchButton.Margin.Left);
                sb.Begin();
            }
            else
            {
                rectangle.Fill = new SolidColorBrush(Colors.White);
                switchButton.Fill = this.BorderBrush;
                Storyboard sb = this.Resources["Storyboard1"] as Storyboard;
                sb.Stop();
                (sb.Children[0] as DoubleAnimationUsingKeyFrames).KeyFrames[0].Value = (0);
                sb.Begin();
            }*/
        }

        public static DependencyProperty SwitchProperty = DependencyProperty.Register("SwitchStatus", typeof(SwitchStatus), typeof(Switch));

        public delegate void SwitchChangedEventHandle(object sender, SwitchChangedEventArgs e);
        public event SwitchChangedEventHandle SwitchChanged;

        [DefaultValue(typeof(SwitchStatus),"Off" )]
        public SwitchStatus SwitchStatus
        {
            get
            {
                return (SwitchStatus)GetValue(SwitchProperty);
            }
            set
            {
                SetValue(SwitchProperty, value);
                if (SwitchChanged != null)
                    SwitchChanged(this, new SwitchChangedEventArgs(value));
                
                if(value == SwitchStatus.On )
                {
                    /*rectangle.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00B9FF"));
                    switchButton.Fill = new SolidColorBrush(Colors.White);*/
                    Storyboard sb = this.Resources["Storyboard1"] as Storyboard;
                    sb.Stop();
                    (sb.Children[0] as DoubleAnimationUsingKeyFrames).KeyFrames[0].Value = (this.ActualWidth - switchButton.ActualWidth);
                    sb.Begin();
                    sb = Resources["SwitchOn"] as Storyboard;
                    sb.Begin();
                }
                else
                {
                    /*rectangle.Fill = new SolidColorBrush(Colors.White);
                    switchButton.Fill = this.BorderBrush;*/
                    Storyboard sb = this.Resources["Storyboard1"] as Storyboard;
                    sb.Stop();
                    (sb.Children[0] as DoubleAnimationUsingKeyFrames).KeyFrames[0].Value = (0);
                    sb.Begin();
                    sb = Resources["SwitchOff"] as Storyboard;
                    sb.Begin();
                }

            }
        }

        public static DependencyProperty ButtonBackgroundBrushProperty = DependencyProperty.Register("ButtonBackgroundBrush", typeof(System.Windows.Media.Brush), typeof(Switch));
        public System.Windows.Media.Brush ButtonBackgroundBrush
        {
            get
            {
                return (System.Windows.Media.Brush)GetValue(ButtonBackgroundBrushProperty);
            }
            set
            {
                SetValue(ButtonBackgroundBrushProperty, value);
            }
        }

        public static DependencyProperty ActiveBrushProperty = DependencyProperty.Register("ActiveBrush", typeof(System.Windows.Media.Brush), typeof(Switch));
        public System.Windows.Media.Brush ActiveBrush
        {
            get
            {
                return (System.Windows.Media.Brush)GetValue(ButtonBackgroundBrushProperty);
            }
            set
            {
                SetValue(ButtonBackgroundBrushProperty, value);
            }
        }

        public static DependencyProperty SwitchBackgroundBrushProperty = DependencyProperty.Register("SwitchBackgroundBrush", typeof(System.Windows.Media.Brush), typeof(Switch));
        public System.Windows.Media.Brush SwitchBackgroundBrush
        {
            get
            {
                return GetValue(SwitchBackgroundBrushProperty) as System.Windows.Media.Brush;
            }
            set
            {
                SetValue(ButtonBackgroundBrushProperty, value);
            }
        }


        public static DependencyProperty BorderWidthProperty = DependencyProperty.Register("BorderWidth", typeof(double), typeof(Switch));
        public double BorderWidth
        {
            get
            {
                return (double)GetValue(BorderWidthProperty);
            }
            set
            {
                SetValue(BorderWidthProperty, value);
            }

        }

        private void userControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void userControl_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (SwitchStatus == SwitchStatus.Off)
                SwitchStatus = SwitchStatus.On;
            else
                SwitchStatus = SwitchStatus.Off;
        }

        private void userControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            e.Handled = true;
        }
    }
}
