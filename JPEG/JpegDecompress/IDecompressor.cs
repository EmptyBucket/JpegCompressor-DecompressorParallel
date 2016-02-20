using System.Drawing;

namespace JPEG.JpegDecompress
{
    public interface IDecompressor
    {
        Bitmap Decompress(CompressedImage compressedImage);
    }
}
