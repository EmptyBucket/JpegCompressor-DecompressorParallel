using System;

namespace JPEG.ArrayToMatrixBuilder
{
    public class ZigZagBuilder : IArrayToMatrixBuilder
    {
        public T[,] Build<T>(T[] array, int matrixLengthX, int matrixLengthY)
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