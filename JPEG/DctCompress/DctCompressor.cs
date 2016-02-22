using System.Linq;
using JPEG.CompressionAlgorithms;
using JPEG.ExtensionsMethods;
using JPEG.Quantification;

namespace JPEG.DctCompress
{
    public class DctCompressor : IDctCompressor
    {
        private readonly IQuantifier _quantifier;
        private const int ShiftIndex = -128;

        public DctCompressor(IQuantifier quantifier)
        {
            _quantifier = quantifier;
        }

        public double[][] Compress(double[,] matrix, int dctSize, int compressionLevel, double[,] matrixQuantification)
        {
            var dctBlocks = matrix
                .DevideIntoBlocks(dctSize, dctSize)
                .AsParallel()
                .Select(block =>
                {
                    var shiftedMatrix = block.ShiftMatrixValues(ShiftIndex);
                    var dctMatrix = Dct.Dct2D(shiftedMatrix);
                    var quantificationMatrix = _quantifier.Quantification(dctMatrix, matrixQuantification)
                        .MatrixZigZagExpand()
                        .Take(compressionLevel)
                        .ToArray();
                    return quantificationMatrix;
                })
                .ToArray();

            return dctBlocks;
        }
    }
}