namespace JPEG.DctCompress
{
    public interface IDctCompressor
    {
        double[][] Compress(double[,] matrix, int dctSize, int compressionLevel);
    }
}