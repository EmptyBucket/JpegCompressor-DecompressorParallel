namespace JPEG.DctCompress
{
    public interface IDctCompressor
    {
        byte[][] Compress(double[,] matrix, int dctSize, int compressionLevel, double[,] matrixQuantification);
    }
}