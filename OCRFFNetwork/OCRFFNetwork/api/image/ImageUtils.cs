using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace OCRFFNetwork.model.api.image
{
    public class ImageUtils
    {

        public static void Resize(string srcPath, int width, int height)
        {
            Image image = Image.FromFile(srcPath);
            Bitmap resultImage = Resize(image, width, height);
            resultImage = BinarizeImage(resultImage);
            resultImage.Save(srcPath.Replace(".png", "_" + width + "x" + height + ".png"));
            resultImage.Dispose();
        }

        public static Bitmap Resize(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public static Bitmap BinarizeImage(Bitmap image)
        {
            int w = image.Width;
            int h = image.Height;
            Bitmap bmp = new Bitmap(w, h, PixelFormat.Format1bppIndexed);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed);
            byte[] scan = new byte[(w + 7) / 8];
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (x % 8 == 0) scan[x / 8] = 0;
                    Color c = image.GetPixel(x, y);
                    if (c.GetBrightness() >= 0.5) scan[x / 8] |= (byte)(0x80 >> (x % 8));
                }
                Marshal.Copy(scan, 0, (IntPtr)((long)data.Scan0 + data.Stride * y), scan.Length);
            }
            bmp.UnlockBits(data);
            return bmp;
        }

        public static ObservableCollection<double> GetImagePixels(string imagePath)
        {
            var imagePixels = new ObservableCollection<double>();

            if (File.Exists(imagePath))
            {
                var bitmapImage = new Bitmap(imagePath);

                for (var i = 0; i < bitmapImage.Width; i++)
                {
                    for (var j = 0; j < bitmapImage.Height; j++)
                    {
                        var pixelColor = bitmapImage.GetPixel(i, j);
                        if (pixelColor.R == 255)
                        {
                            imagePixels.Add(0);
                        }
                        else if (pixelColor.R == 0)
                        {
                            imagePixels.Add(1);
                        }
                    }
                }
            }

            return imagePixels;
        }

    }

}

