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
        public List<SKBitmap> OriginImages = new List<SKBitmap>();
        public List<SKBitmap> Images = new List<SKBitmap>();
        public SpriteSheetData SpriteSheetData;
        public int MaxWidth => Images.Max(img => img.Width);
        public int MaxHeight => Images.Max(img => img.Height);
        public int Columns => (int)Math.Ceiling(Math.Sqrt((double)Images.Count * MaxHeight / MaxWidth));
        public bool ClipTransparent = false;

        public event Action<double> OnProgress;

        public SpriteSheetGenerator(string[] paths)
        {
            Paths = paths;// paths.OrderBy(p => p).ToArray();
        }

        public void Load()
        {
            OriginImages = new List<SKBitmap>();
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
                    OriginImages.Add(bitmap);
                    OnProgress?.Invoke((double)(i + 1) / Paths.Length);

                }
            }
            PostProcess();
        }

        public void PostProcess()
        {
            for (var i = 0; i < Images.Count; i++)
                if (Images[i] != OriginImages[i])
                    Images[i].Dispose();

            

            if (!ClipTransparent)
            {
                Images = OriginImages;
                return;
            }

            var clipList = new SKRectI[OriginImages.Count];

            for(var i = 0; i < OriginImages.Count; i++)
            {
                var bitmap = OriginImages[i];
                var clip = new SKRectI();
                if (ClipTransparent)
                {
                    // Clip X
                    for(var x = 0; x < bitmap.Width; x++)
                    {
                        if (!ColumnTransparentScan(bitmap, x))
                        {
                            clip.Left = x;
                            break;
                        }
                    }
                    for(var x = bitmap.Width - 1; x >= 0; x--)
                    {
                        if (!ColumnTransparentScan(bitmap, x))
                        {
                            clip.Right = x + 1;
                            break;
                        }
                    }

                    // Clip Y
                    for(var y = 0; y < bitmap.Height; y++)
                    {
                        if (!RowTransparentScan(bitmap, y))
                        {
                            clip.Top = y;
                            break;
                        }
                    }
                    for(var y = bitmap.Height - 1; y >= 0; y--)
                    {
                        if(!RowTransparentScan(bitmap,y))
                        {
                            clip.Bottom = y + 1;
                            break;
                        }
                    }
                }
                clipList[i] = clip;
            }
            var minX = clipList.Select(rect => rect.Left).Min();
            var minY = clipList.Select(rect => rect.Top).Min();
            Images = new List<SKBitmap>();
            for (var i = 0; i < OriginImages.Count; i++)
            {
                var clip = clipList[i];
                clip.Left = minX;
                clip.Top = minY;
                var bitmap = new SKBitmap(clip.Width, clip.Height, SKColorType.Bgra8888, SKAlphaType.Premul);
                using (var canvas = new SKCanvas(bitmap))
                {
                    canvas.Clear();
                    canvas.DrawBitmap(OriginImages[i], clip, new SKRectI(0, 0, clip.Width, clip.Height));
                    canvas.Flush();
                    canvas.Dispose();
                }
                Images.Add(bitmap);
            }
        }

        private bool ColumnTransparentScan(SKBitmap bitmap, int column)
        {
            var pixels = bitmap.Pixels;
            for (var y = 0; y< bitmap.Height; y++)
            {
                if (pixels[y * bitmap.Width + column].Alpha > 0)
                    return false;
            }
            return true;
        }

        private bool RowTransparentScan(SKBitmap bitmap, int row)
        {
            var pixels = bitmap.Pixels;
            for(var x = 0; x < bitmap.Width; x++)
            {
                if (pixels[row * bitmap.Width + x].Alpha > 0)
                    return false;
            }
            return true;
        }

        public SKBitmap Render(int width, int height)
        {
            var w = width / MaxWidth;
            var h = height / MaxHeight;
            var bitmap = new SKBitmap(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);
            this.SpriteSheetData = new SpriteSheetData();
            this.SpriteSheetData.sprites = new SpriteData[OriginImages.Count];
            using (var canvas = new SKCanvas(bitmap))
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
                        SpriteSheetData.sprites[idx] = new SpriteData()
                        {
                            x = x * MaxWidth,
                            y = (h - 1 - y) * MaxHeight,
                            width = MaxWidth,
                            height = MaxHeight,
                            pivotX = MaxWidth / 2,
                            pivotY = MaxHeight / 2
                        };
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
                canvas.Clear();
                canvas.DrawBitmap(img, 0, 0);
                canvas.Flush();
            }
            bitmap.AddDirtyRect(new System.Windows.Int32Rect(0, 0, width, height));
            bitmap.Unlock();
            return bitmap;
        }
    }
}
