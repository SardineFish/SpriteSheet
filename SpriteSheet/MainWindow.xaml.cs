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
using System.IO;
using Microsoft.Win32;

namespace SpriteSheet
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public const int ScrollUnit = 32;
        public const double SpriteLoadProgress = 0.8;
        public const double PreviewLoadProgress = 0.2;
        public SpriteSheetGenerator SpriteSheet;
        Task SpriteTask;
        Brush TranspatentBG;
        bool dragHold = false;
        Vector RenderImageSize;
        Vector PreviewImageSize;
        int Columns = 0;
        int Rows = 0;
        public MainWindow()
        {
            InitializeComponent();
            
        }

        public async void Update()
        {
            workSpace.Visibility = Visibility.Visible;
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

            for(var i=0;i<SpriteSheet.Images.Count;i++)
            {
                var img = SpriteSheet.Images[i];

                var grid = new Grid();
                grid.Width = SpriteSheet.MaxWidth * scale;
                grid.Height = SpriteSheet.MaxHeight * scale;
                var image = new Image();

                /*var task = new Task<WriteableBitmap>(() => SpriteSheetGenerator.ToWriteableBitmap(img, SpriteSheet.MaxWidth, SpriteSheet.MaxHeight));
                task.Start();*/
                image.Source = SpriteSheetGenerator.ToWriteableBitmap(img, SpriteSheet.MaxWidth, SpriteSheet.MaxHeight);

                image.SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.HighQuality);
                image.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Unspecified);
                image.SnapsToDevicePixels = true;
                image.Width = SpriteSheet.MaxWidth * scale;
                image.Height = SpriteSheet.MaxHeight * scale;
                image.Stretch = Stretch.Fill;
                grid.Children.Add(image);
                images.Children.Add(grid);

                progressBar.Value  =100*PreviewLoadProgress * ((double)(i+1)/SpriteSheet.Images.Count) + 100*(1-PreviewLoadProgress);
            }

            await Task.Delay(500);
            progressBar.Value = 0;
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

        private async void editorWrapper_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var paths = (e.Data.GetData(DataFormats.FileDrop) as string[]);
                SpriteSheet = new SpriteSheetGenerator(paths);
                SpriteSheet.OnProgress += SpriteSheet_OnProgress;
                SpriteTask = new Task(()=>
                {
                    SpriteSheet.Load();
                });
                SpriteTask.Start();
                await SpriteTask;
                Update();
                textPath.Text = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(paths[0]), "SpriteSheet.png");
            }
        }

        private void SpriteSheet_OnProgress(double progress)
        {
            this.Dispatcher.Invoke(() =>
            {
                progressBar.Value = 100 * SpriteLoadProgress * progress;
            });
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

        private void dropBackground_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TranspatentBG == null)
                return;
            switch (dropBackground.SelectedIndex)
            {
                case 0:
                    images.Background = TranspatentBG;
                    break;
                case 1:
                    images.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FEFEFE"));
                    break;
                case 2:
                    images.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3c3c3c"));
                    break;
            }
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            TranspatentBG = images.Background;
            workSpace.Visibility = Visibility.Collapsed;
        }

        private void workSpace_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scroll = Math.Sign(e.Delta) * ScrollUnit;
            if(switchMultiRow.SwitchStatus == SwitchStatus.On)
            {
                imagesTransform.Y += scroll;
            }
            else
            {
                imagesTransform.X += scroll;
            }
        }

        private void buttonReset_Click(object sender, RoutedEventArgs e)
        {
            workSpace.Visibility = Visibility.Collapsed;
        }

        private void buttonBrowse_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.InitialDirectory = System.IO.Path.GetDirectoryName(textPath.Text);
            saveFile.Filter = "Image Files(*.png)|*.png";
            saveFile.FileName = "SpriteSheet.png";
            saveFile.ShowDialog();
            textPath.Text = saveFile.FileName;
        }

        private async void buttonExport_Click(object sender, RoutedEventArgs e)
        {
            var task = new Task<SkiaSharp.SKBitmap>(() => SpriteSheet.Render((int)RenderImageSize.X, (int)RenderImageSize.Y));
            task.Start();
            var bitmap = await task;
            using (var fs = new FileStream(textPath.Text, FileMode.Create))
            using (var sks = new SkiaSharp.SKManagedWStream(fs))
            {
                SkiaSharp.SKPixmap.Encode(sks, bitmap, SkiaSharp.SKEncodedImageFormat.Png, 100);
                sks.Flush();
                fs.Close();
            }
            progressBar.Value = 100;

            await Task.Delay(500);
            progressBar.Value = 0;
        }
    }
}
