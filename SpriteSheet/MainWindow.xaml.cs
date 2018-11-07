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

namespace SpriteSheet
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        bool dragHold = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void root_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed && dragHold)
                this.DragMove();
        }

        private void buttonMin_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void root_MouseDown(object sender, MouseButtonEventArgs e) => dragHold = true;

        private void root_MouseUp(object sender, MouseButtonEventArgs e) => dragHold = false;

        private void editorWrapper_MouseDown(object sender, MouseButtonEventArgs e) => e.Handled = true;
    }
}
