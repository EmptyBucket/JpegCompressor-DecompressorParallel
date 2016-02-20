using System.Drawing;
using System.Drawing.Imaging;
using JPEG.Pixel;

namespace JPEG.BitmapBuild
{
    public interface IBitmapBuilder
    {
        Bitmap Build(RgbPixel[,] matrix, PixelFormat pixelFormat);
    }
}
