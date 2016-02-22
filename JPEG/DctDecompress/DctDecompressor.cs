using System.Collections.Generic;
using System.Linq;
using JPEG.CompressionAlgorithms;
using JPEG.ExtensionsMethods;
using JPEG.Quantification;

namespace JPEG.DctDecompress
{
    public class DctDecompressor : IDctDecompressor
    {
        private readonly IQuantifier _quantifier;
        private const int ShiftIndex = -128;

        public DctDecompressor(IQuantifier quantifier)
        {
            _quantifier = quantifier;
        }

        private static T[] AdditionBlock<T>(IReadOnlyCollection<T> block, int countAddValue) => block
            .Concat(Enumerable.Repeat(default(T), countAddValue))
            .ToArray();

        public double[,] Decompress(double[][] blocks, int dctSize, int imageHeight, int imageWidth, double[,] matrixQuantification)
        {
            var result = blocks
                .AsParallel()
                .Select(block =>
                {
                    var blockShiftValue = block.ShiftArrayValue(ShiftIndex);
                    var additionBlock = AdditionBlock(blockShiftValue, dctSize*dctSize - block.Length);
                    var matrix = additionBlock.MatrixZigZagTurn(dctSize, dctSize);
                    var unQuantifiedMatrix = _quantifier.UnQuantification(matrix, matrixQuantification);
                    var unDctMatrix = Dct.Idct2D(unQuantifiedMatrix);
                    return unDctMatrix;
                })
                .ToArray()
                .MatrixBuildFromBlocks(imageHeight, imageWidth);
            return result;
        }
    }
}