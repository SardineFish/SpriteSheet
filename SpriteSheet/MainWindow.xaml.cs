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
using SardineFish.Windows.Controls;

namespace SpriteSheet
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public SpriteSheetGenerator SpriteSheet;
        bool dragHold = false;
        Vector RenderImageSize;
        Vector PreviewImageSize;
        int Columns = 0;
        int Rows = 0;
        public MainWindow()
        {
            InitializeComponent();
        }

        public void Update()
        {
            Columns = SpriteSheet.Columns;
            if (switchMultiRow.SwitchStatus == SardineFish.Windows.Controls.SwitchStatus.Off)
                Columns = SpriteSheet.Images.Count;
            Rows = (int)Math.Ceiling((double)SpriteSheet.Images.Count / Columns);

            RenderImageSize = new Vector(Columns * SpriteSheet.MaxWidth, Rows * SpriteSheet.MaxHeight);
            if (switchSnap2Pow.SwitchStatus == SwitchStatus.On)
            {
                var size = (int)Math.Pow(2, Math.Ceiling(Math.Log(RenderImageSize.X, 2)));
                RenderImageSize = new Vector(size, size);
            }
            PreviewImageSize = RenderImageSize;

            images.Children.Clear();
            double scale = 1;

            if(switchMultiRow.SwitchStatus == SwitchStatus.On)
            {
                if (PreviewImageSize.X> workSpace.ActualWidth - 64)
                {
                    scale = (workSpace.ActualWidth - 64) / RenderImageSize.X;
                    PreviewImageSize *= scale;
                }
            }
            else
            {
                if (PreviewImageSize.Y > workSpace.ActualHeight - 128)
                {
                    scale = (workSpace.ActualHeight - 128) / RenderImageSize.Y;
                    PreviewImageSize *= scale;
                }
            }
            images.Width = PreviewImageSize.X;
            images.Height = PreviewImageSize.Y;

            var centerOffset = new Vector(workSpace.ActualWidth - PreviewImageSize.X, workSpace.ActualHeight - PreviewImageSize.Y);
            centerOffset /= 2;
            if (switchMultiRow.SwitchStatus == SwitchStatus.On && PreviewImageSize.Y > workSpace.ActualHeight)
            {
                centerOffset.Y += (PreviewImageSize.Y - workSpace.ActualHeight) / 2 + 32;
            }
            else if(switchMultiRow.SwitchStatus == SwitchStatus.Off && PreviewImageSize.X > workSpace.ActualWidth)
            {
                centerOffset.X += (PreviewImageSize.X - workSpace.ActualWidth) / 2 + 64;
            }
            imagesTransform.X = centerOffset.X;
            imagesTransform.Y = centerOffset.Y;

            foreach (var img in SpriteSheet.Images)
            {
                var grid = new Grid();
                grid.Width = SpriteSheet.MaxWidth * scale;
                grid.Height = SpriteSheet.MaxHeight * scale;
                var image = new Image();
                image.Source = SpriteSheetGenerator.ToWriteableBitmap(img, SpriteSheet.MaxWidth, SpriteSheet.MaxHeight);
                image.SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.HighQuality);
                image.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Unspecified);
                image.SnapsToDevicePixels = true;
                image.Width = SpriteSheet.MaxWidth * scale;
                image.Height = SpriteSheet.MaxHeight * scale;
                image.Stretch = Stretch.Fill;
                grid.Children.Add(image);
                images.Children.Add(grid);
            }
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

        private void editorWrapper_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Move;
            else
                e.Effects = DragDropEffects.None;
        }

        private void editorWrapper_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var paths = (e.Data.GetData(DataFormats.FileDrop) as string[]);
                SpriteSheet = new SpriteSheetGenerator(paths);
                SpriteSheet.Load();
                Update();
                
            }
        }

        private void switchMultiRow_SwitchChanged(object sender, EventArgs e)
        {
            if (switchMultiRow.SwitchStatus == SwitchStatus.Off)
                switchSnap2Pow.SwitchStatus = SwitchStatus.Off;
            Update();
        }

        private void switchReverse_SwitchChanged(object sender, EventArgs e) => Update();

        private void switchSnap2Pow_SwitchChanged(object sender, EventArgs e)
        {
            if (switchSnap2Pow.SwitchStatus == SwitchStatus.On)
                switchMultiRow.SwitchStatus = SwitchStatus.On;
            Update();
        }
    }
}
