﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JPEG.ArrayToMatrixBuilder;
using JPEG.CompressionAlgorithms;
using JPEG.ExtensionsMethods;
using JPEG.Quantification;

namespace JPEG.DctDecompress
{
    public class DctDecompressor : IDctDecompressor
    {
        private readonly IQuantifier _quantifier;
        private readonly IArrayToMatrixBuilder _arrayToMatrixBuilder;
        private const int ShiftIndex = -128;

        public DctDecompressor(IQuantifier quantifier, IArrayToMatrixBuilder arrayToMatrixBuilder)
        {
            _quantifier = quantifier;
            _arrayToMatrixBuilder = arrayToMatrixBuilder;
        }

        private static T[] AdditionBlock<T>(IEnumerable<T> block, int countAddValue) => block
            .Concat(Enumerable.Repeat(default(T), countAddValue))
            .ToArray();

        public double[,] Decompress(double[][] blocks, int dctSize, int imageHeight, int imageWidth, double[,] matrixQuantification)
        {
            var result = blocks
                .AsParallel()
                .AsOrdered()
                .Select((block, index) =>
                {
                    Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " " + index);
                    var blockShiftValue = block.ShiftArrayValue(ShiftIndex);
                    var additionBlock = AdditionBlock(blockShiftValue, dctSize*dctSize - block.Length);
                    var matrix = _arrayToMatrixBuilder.Build(additionBlock, dctSize, dctSize);
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