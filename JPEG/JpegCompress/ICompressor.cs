using System.Drawing;

namespace JPEG.JpegCompress
{
    public interface ICompressor
    {
        CompressedImage Compress(Bitmap image);
    }
}