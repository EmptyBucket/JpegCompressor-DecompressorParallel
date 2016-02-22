using System;
using System.Linq;
using JPEG.CompressionAlgorithms;
using JPEG.ExtensionsMethods;
using JPEG.Quantification;

namespace JPEG.DctCompress
{
    public class DctCompressor : IDctCompressor
    {
        private readonly IQuantifier _quantifier;
        private const int ShiftIndex = 128;

        public DctCompressor(IQuantifier quantifier)
        {
            _quantifier = quantifier;
        }

        public byte[][] Compress(double[,] matrix, int dctSize, int compressionLevel, double[,] matrixQuantification)
        {
            var dctBlocks = matrix
                .DevideIntoBlocks(dctSize, dctSize)
                .AsParallel()
                .Select(block =>
                {
                    var dctMatrix = Dct.Dct2D(block);
                    var quantificationMatrix = _quantifier.Quantification(dctMatrix, matrixQuantification);
                    var blockShiftValue = quantificationMatrix
                        .MatrixZigZagExpand()
                        .Take(compressionLevel)
                        .Select(shiftBlock => (byte)Math.Round(shiftBlock))
                        .ToArray()
                        .ShiftArrayValue(ShiftIndex);
                    return blockShiftValue;
                })
                .ToArray();

            return dctBlocks;
        }
    }
}