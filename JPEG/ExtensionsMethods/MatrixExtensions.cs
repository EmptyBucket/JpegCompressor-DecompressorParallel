using System;
using System.Collections.Generic;
using System.Linq;
using Ninject.Infrastructure.Language;

namespace JPEG.ExtensionsMethods
{
    public static class MatrixExtensions
    {
        public static double[,] ShiftMatrixValues(this double[,] matrix, double shiftValue)
        {
            var height = matrix.GetLength(0);
            var width = matrix.GetLength(1);
            var shuftedValueMatrix = new double[height, width];
            for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                    shuftedValueMatrix[y,x] = matrix[y, x] + shiftValue;
            return shuftedValueMatrix;
        }

        public static void SetSubmatrix<T>(this T[,] destination, T[,] source, int yOffset, int xOffset)
        {
            if(yOffset < 0 || xOffset < 0)
                throw new Exception("Received a negative bias");
            if(yOffset+source.Length > destination.Length || xOffset+source.Length > destination.Length)
                throw new Exception("Output sub-matrix of matrix");
            var height = source.GetLength(0);
            var width = source.GetLength(1);
            for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                    destination[yOffset + y, xOffset + x] = source[y, x];
        }

        public static T[,] GetSubMatrix<T>(this T[,] matrix, int yOffset, int yLength, int xOffset, int xLength)
        {
            if(xLength < 0 || yLength < 0)
                throw new Exception("Received a negative length");
            if(yOffset < 0 || xOffset < 0)
                throw new Exception("Received a negative bias");
            if(yOffset+yLength > matrix.Length || xOffset+xLength > matrix.Length)
                throw new Exception("Output sub-matrix of matrix");
            var result = new T[yLength, xLength];
            for (var j = 0; j < yLength; j++)
                for (var i = 0; i < xLength; i++)
                    result[j, i] = matrix[yOffset + j, xOffset + i];
            return result;
        }

        public static T[][,] DevideIntoBlocks<T>(this T[,] matrix, int xLengthBlock, int yLengthBlock)
        {   
            var height = matrix.GetLength(0);
            var width = matrix.GetLength(1);
            var result = new List<T[,]>();
            for (var y = 0; y < height; y += yLengthBlock)
                for (var x = 0; x < width; x += xLengthBlock)
                {
                    var subMatrix = matrix.GetSubMatrix(y, yLengthBlock, x, xLengthBlock);
                    result.Add(subMatrix);
                }
            return result.ToArray();
        }

        public static T[,] MatrixBuildFromBlocks<T>(this T[][,] blocks, int sizeY, int sizeX)
        {
            var heightBlock = blocks[0].GetLength(0);
            var widthBlock = blocks[0].GetLength(1);
            if(sizeY*sizeX != blocks.Length*heightBlock*widthBlock)
                throw new Exception("The matrix size is not equal to the size of cells");
            var matrix = new T[sizeY, sizeX];
            var enumeratorBlocks = blocks.ToEnumerable().GetEnumerator();
            for (var y = 0; y < sizeY; y += heightBlock)
                for (var x = 0; x < sizeX; x += widthBlock)
                {
                    enumeratorBlocks.MoveNext();
                    var block = enumeratorBlocks.Current;
                    matrix.SetSubmatrix(block, y, x);
                }
            return matrix;
        }

        public static T[] MatrixZigZagExpand<T>(this T[,] matrix)
        {
            var listStart = new List<T>();
            var listEnd = new List<T>();

            var lengthY = matrix.GetLength(0);
            var lengthX = matrix.GetLength(1);
            var i = 0;
            var j = 0;
            var directionStep = -1;
            var start = 0;
            var end = lengthY * lengthX - 1;
            do
            {
                listStart.Add(matrix[i, j]);
                start++;
                listEnd.Add(matrix[lengthY - i - 1, lengthX - j - 1]);
                end--;

                i += directionStep;
                j -= directionStep;
                if (i < 0)
                {
                    i++;
                    directionStep = -directionStep;
                }
                else if (j < 0)
                {
                    j++;
                    directionStep = -directionStep;
                }
            } while (start < end);
            if (start == end)
                listStart.Add(matrix[j,i]);
            listEnd.Reverse();
            var commonList = listStart.Concat(listEnd);
            return commonList.ToArray();
        }
    }
}