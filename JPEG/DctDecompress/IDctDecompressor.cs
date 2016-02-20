namespace JPEG.DctDecompress
{
    public interface IDctDecompressor
    {
        double[,] Decompress(double[][] blocks, int dctSize, int imageHeight, int imageWidth);
    }
}