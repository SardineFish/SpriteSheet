using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace SpriteSheet
{
    public class SpriteSheetGenerator
    {
        string[] Paths;
        public List<SKBitmap> Images = new List<SKBitmap>();
        public int MaxWidth => Images.Max(img => img.Width);
        public int MaxHeight => Images.Max(img => img.Height);
        public int Columns => (int)Math.Ceiling(Math.Sqrt((double)Images.Count * MaxHeight / MaxWidth));

        public SpriteSheetGenerator(string[] paths)
        {
            Paths = paths.OrderBy(p => p).ToArray();
        }

        public void Load()
        {
            foreach (var path in Paths)
            {
                SKBitmap bitmap = null;

                using (var fs = new FileStream(path, FileMode.Open))
                {
                    bitmap = SKBitmap.Decode(fs);
                    fs.Close();
                }
                Images.Add(bitmap);
            }
        }

        // Reference http://lostindetails.com/blog/post/SkiaSharp-with-Wpf
        public static WriteableBitmap ToWriteableBitmap(SKBitmap img, int width, int height)
        {
            var bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, BitmapPalettes.Halftone256Transparent);
            bitmap.Lock();
            using (var surface = SKSurface.Create(width:width,height:height,colorType:SKColorType.Bgra8888, alphaType:SKAlphaType.Premul,pixels:bitmap.BackBuffer,rowBytes: width * 4))
            {
                var canvas = surface.Canvas;
                canvas.DrawBitmap(img, 0, 0);
            }
            bitmap.AddDirtyRect(new System.Windows.Int32Rect(0, 0, width, height));
            bitmap.Unlock();
            return bitmap;
        }
    }
}
