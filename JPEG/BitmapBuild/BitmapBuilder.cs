using System.Drawing;
using System.Drawing.Imaging;
using JPEG.Pixel;

namespace JPEG.BitmapBuild
{
    public class BitmapBuilder : IBitmapBuilder
    {
        public Bitmap Build(RgbPixel[,] matrix, PixelFormat pixelFormat)
        {
            var result = new Bitmap(matrix.GetLength(1), matrix.GetLength(0), pixelFormat);
            var bitmapData = result.LockBits(new Rectangle(0, 0, result.Width, result.Height), ImageLockMode.ReadOnly, pixelFormat);
            const int rgpPixelSize = 3;
            var additional = bitmapData.Stride - bitmapData.Width*rgpPixelSize;

            unsafe
            {
                var imagePointer = (byte*)bitmapData.Scan0;
                for (var j = 0; j < bitmapData.Height; j++,imagePointer += additional)
                    for (var i = 0; i < bitmapData.Width; i++, imagePointer += rgpPixelSize)
                    {
                        imagePointer[0] = matrix[j, i].B;
                        imagePointer[1] = matrix[j, i].G;
                        imagePointer[2] = matrix[j, i].R;
                    }
            }
            result.UnlockBits(bitmapData);

            return result;
        }
    }
}
