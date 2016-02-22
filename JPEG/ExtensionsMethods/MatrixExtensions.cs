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
            var newMatrix = new double[height, width];
            for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                    newMatrix[y,x] = matrix[y, x] + shiftValue;
            return newMatrix;
        }

        public static double[] ShiftArrayValue(this double[] array, double shiftValue) =>
            array.Select(item => item + shiftValue).ToArray();

        public static byte[] ShiftArrayValue(this byte[] array, int shiftValue) =>
            array.Select(item => (byte)(item + shiftValue)).ToArray();

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

        public static T[,] GetSubMatrix<T>(this T[,] array, int yOffset, int yLength, int xOffset, int xLength)
        {
            if(xLength < 0 || yLength < 0)
                throw new Exception("Received a negative length");
            if(yOffset < 0 || xOffset < 0)
                throw new Exception("Received a negative bias");
            if(yOffset+yLength > array.Length || xOffset+xLength > array.Length)
                throw new Exception("Output sub-matrix of matrix");
            var result = new T[yLength, xLength];
            for (var j = 0; j < yLength; j++)
                for (var i = 0; i < xLength; i++)
                    result[j, i] = array[yOffset + j, xOffset + i];
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

        public static T[,] MatrixZigZagTurn<T>(this T[] array, int matrixLengthX, int matrixLengthY)
        {
            if (array.Length > matrixLengthX*matrixLengthY)
                throw new Exception("Size of the matrix is less than the array size");
            var matrix = new T[matrixLengthY, matrixLengthX];
            var i = 0;
            var j = 0;
            var directionStep = -1;
            var start = 0;
            var end = matrixLengthX * matrixLengthY - 1;
            do
            {
                matrix[i, j] = array[start++];
                matrix[matrixLengthX - i - 1, matrixLengthX - j - 1] = array[end--];

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
                matrix[j, i] = array[start];
            return matrix;
        }
    }
}