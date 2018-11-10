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

        public event Action<double> OnProgress;

        public SpriteSheetGenerator(string[] paths)
        {
            Paths = paths.OrderBy(p => p).ToArray();
        }

        public void Load()
        {
            for(var i = 0; i < Paths.Length; i++)
            {
                var path = Paths[i];

                SKBitmap bitmap = null;

                using (var ms = new MemoryStream())
                {
                    using (var fs = new FileStream(path, FileMode.Open))
                    {
                        fs.CopyTo(ms);
                        ms.Flush();
                        fs.Close();
                    }

                    ms.Position = 0;
                    bitmap = SKBitmap.Decode(ms);
                    Images.Add(bitmap);
                    OnProgress?.Invoke((double)(i + 1) / Paths.Length);

                }

                /*
                using (var fs = new FileStream(path, FileMode.Open))
                {
                    bitmap = SKBitmap.Decode(fs);
                    fs.Close();
                }
                Images.Add(bitmap);
                OnProgress?.Invoke((double)(i + 1) / Paths.Length);*/
            }
        }

        public SKBitmap Render(int width, int height)
        {
            var w = width / MaxWidth;
            var h = height / MaxHeight;
            var bitmap = new SKBitmap(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);
            using(var canvas = new SKCanvas(bitmap))
            {
                canvas.Clear();
                for (var y = 0; y < h; y++)
                {
                    for (var x = 0; x < w; x++)
                    {
                        var idx = y * w + x;
                        if (idx >= Images.Count)
                            goto End;
                        canvas.DrawBitmap(Images[idx], x * MaxWidth, y * MaxHeight);
                        OnProgress?.Invoke((double)(idx + 1) / Images.Count);
                    }
                }
                End:
                canvas.Flush();
                canvas.Dispose();
            }
            return bitmap;
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
