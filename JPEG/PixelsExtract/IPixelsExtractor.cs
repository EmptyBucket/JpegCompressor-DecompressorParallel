using System.Drawing;

namespace JPEG.PixelsExtract
{
    public interface IPixelsExtractor<out T>
    {
        T[,] Extract(Bitmap bitmap);
    }
}
