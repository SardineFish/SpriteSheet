using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using Microsoft.Win32;

namespace ImageSequence
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            List<Bitmap> bmpList = new List<Bitmap>();
            var sliceWidth = 0;
            var sliceHeight = 0;
            if (args.Length <= 0)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = true;
                openFileDialog.Filter = "Image|*.bmp;*.jpg;*.png|All file|*.*";
                openFileDialog.ShowDialog();
                foreach (var path in openFileDialog.FileNames)
                {
                    var bmp = new Bitmap(path);
                    sliceWidth = Math.Max(sliceWidth, bmp.Width);
                    sliceHeight = Math.Max(sliceHeight, bmp.Height);
                    bmpList.Add(new Bitmap(path));
                }
            }
            var imgSeq = new Bitmap(sliceWidth * bmpList.Count, sliceHeight,PixelFormat.Format32bppArgb);
            
            Graphics gdi = Graphics.FromImage(imgSeq);
            gdi.Clear(Color.Transparent);
            for(var i = 0; i < bmpList.Count; i++)
            {
                gdi.DrawImage(bmpList[i], sliceWidth * i, 0);
                gdi.Save();
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "Image Sequence.png";
            saveFileDialog.ShowDialog();
            imgSeq.Save(saveFileDialog.FileName, ImageFormat.Png);
            //Bitmap bmp = new Bitmap ()
        }
    }
}
