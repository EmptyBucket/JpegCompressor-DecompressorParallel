using System.Linq;
using JPEG.CompressionAlgorithms;
using JPEG.ExtensionsMethods;

namespace JPEG.DctCompress
{
    public class DctCompressor : IDctCompressor
    {
        private const int ShiftIndex = -128;

        public double[][] Compress(double[,] matrix, int dctSize, int compressionLevel)
        {
            var dctBlocks = matrix.DevideIntoBlocks(dctSize, dctSize)
                .AsParallel()
                .Select(block => Dct.Dct2D(block.ShiftMatrixValues(ShiftIndex)).MatrixZigZagExpand().Take(compressionLevel).ToArray())
                .ToArray();

            return dctBlocks;
        }
    }
}