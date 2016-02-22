using System;
using System.Linq;

namespace JPEG.ExtensionsMethods
{
    public static class ArrayExtensions
    {
         public static double[] ShiftArrayValue(this double[] array, double shiftValue) =>
            array.Select(item => item + shiftValue).ToArray();

        public static byte[] ShiftArrayValue(this byte[] array, int shiftValue) =>
            array.Select(item => (byte)(item + shiftValue)).ToArray();

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