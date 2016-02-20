using System.Drawing;
using System.Drawing.Imaging;
using JPEG.Pixel;

namespace JPEG.PixelsExtract
{
    public class BgrPixelsExtractor : IPixelsExtractor<RgbPixel>
    {
        public RgbPixel[,] Extract(Bitmap bmp)
        {
            var bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
            const int rgpPixelSize = 3;
            var additional = bitmapData.Stride - bitmapData.Width*rgpPixelSize;

            var result = new RgbPixel[bmp.Height, bmp.Width];

            unsafe
            {
                var imagePointer = (byte*)bitmapData.Scan0;
                for (var j = 0; j < bitmapData.Height; j++, imagePointer += additional)
                    for (var i = 0; i < bitmapData.Width; i++, imagePointer += rgpPixelSize)
                        result[j, i] = new RgbPixel(imagePointer[2], imagePointer[1], imagePointer[0]);
            }
            bmp.UnlockBits(bitmapData);
            return result;
        }
    }
}