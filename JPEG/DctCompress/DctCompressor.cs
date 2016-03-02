using System;
using System.Linq;
using JPEG.CompressionAlgorithms;
using JPEG.ExtensionsMethods;
using JPEG.MatrixToArrayTransform;
using JPEG.Quantification;

namespace JPEG.DctCompress
{
    public class DctCompressor : IDctCompressor
    {
        private readonly IQuantifier _quantifier;
        private readonly IMatrixToArrayTransformer _matrixToArrayTransformer;
        private const int ShiftIndex = 128;

        public DctCompressor(IQuantifier quantifier, IMatrixToArrayTransformer matrixToArrayTransformer)
        {
            _quantifier = quantifier;
            _matrixToArrayTransformer = matrixToArrayTransformer;
        }

        public byte[][] Compress(double[,] matrix, int dctSize, int compressionLevel, double[,] matrixQuantification)
        {
            var dctBlocks = matrix
                .DevideIntoBlocks(dctSize, dctSize)
                .AsParallel()
                .AsOrdered()
                .Select(block =>
                {
                    var dctMatrix = Dct.Dct2D(block);
                    var quantificationMatrix = _quantifier.Quantification(dctMatrix, matrixQuantification);
                    var transformedMatrix = _matrixToArrayTransformer.Transform(quantificationMatrix);
                    var blockShiftValue = transformedMatrix
                        .Take(compressionLevel)
                        .Select(shiftBlock => (byte)Math.Round(shiftBlock))
                        .ShiftArrayValue(ShiftIndex)
                        .ToArray();
                    return blockShiftValue;
                })
                .ToArray();

            return dctBlocks;
        }
    }
}