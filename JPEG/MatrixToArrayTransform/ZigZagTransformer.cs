using System.Collections.Generic;
using System.Linq;

namespace JPEG.MatrixToArrayTransform
{
    public class ZigZagTransformer : IMatrixToArrayTransformer
    {
        public T[] Transform<T>(T[,] matrix)
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