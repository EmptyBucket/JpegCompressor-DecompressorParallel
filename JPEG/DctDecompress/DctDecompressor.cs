using System.Linq;
using JPEG.CompressionAlgorithms;
using JPEG.ExtensionsMethods;

namespace JPEG.DctDecompress
{
    public class DctDecompressor : IDctDecompressor
    {
        private const int ShiftIndes = 128;

        public double[,] Decompress(double[][] blocks, int dctSize, int imageHeight, int imageWidth)
        {
            var matrix = blocks
                .AsParallel()
                .Select(block => Dct.Idct2D(block
                                        .Concat(Enumerable.Repeat(default(double), dctSize * dctSize - block.Length))
                                        .ToArray()
                                        .MatrixZigZagTurn(dctSize, dctSize))
                                    .ShiftMatrixValues(ShiftIndes))
                                    .ToArray()
                .MatrixBuildFromBlocks(imageHeight, imageWidth);
            return matrix;
        }
    }
}