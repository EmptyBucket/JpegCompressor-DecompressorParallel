using System.Drawing;
using JPEG.Pixel;

namespace JPEG.BitmapBuild
{
    public interface IBitmapBuilder
    {
        Bitmap Build(RgbPixel[,] matrixPixels);
    }
}
