using System.Drawing;
using System.Drawing.Imaging;
using JPEG.ExtensionsMethods;
using JPEG.Pixel;

namespace JPEG.BitmapBuild
{
    public class BgrBitmapBuilder : IBitmapBuilder
    {
        public Bitmap Build(RgbPixel[,] matrixPixels)
        {
            const PixelFormat pixelFormat = PixelFormat.Format24bppRgb;
            var colorDepth = Image.GetPixelFormatSize(pixelFormat);
            var pixelSize = BitsCountConverter.ToCoutBytes(colorDepth);
            var bmp = new Bitmap(matrixPixels.GetLength(1), matrixPixels.GetLength(0), pixelFormat);
            var bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, pixelFormat);
            var additionalRow = bitmapData.Stride - bitmapData.Width*pixelSize;

            unsafe
            {
                var imagePointer = (byte*)bitmapData.Scan0;
                for (var j = 0; j < bitmapData.Height; j++,imagePointer += additionalRow)
                    for (var i = 0; i < bitmapData.Width; i++, imagePointer += pixelSize)
                    {
                        imagePointer[0] = matrixPixels[j, i].B;
                        imagePointer[1] = matrixPixels[j, i].G;
                        imagePointer[2] = matrixPixels[j, i].R;
                    }
            }
            bmp.UnlockBits(bitmapData);

            return bmp;
        }
    }
}
