using JPEG.ExtensionsMethods;

namespace JPEG.MatrixExtend
{
    public class DuplicateMatrixExtender<T> : IMatrixExtender<T>
    {
        public T[,] Extend(T[,] matrix, int extendIndex)
        {
            var height = matrix.GetLength(0);
            var width = matrix.GetLength(1);
            var extendedMatrix = new T[height*extendIndex, width*extendIndex];
            for (var i = 0; i < height; i++)
                for (var j = 0; j < width; j++)
                {
                    var subMatrix = new T[extendIndex, extendIndex];
                    for (var k = 0; k < extendIndex; k++)
                        for (var l = 0; l < extendIndex; l++)
                            subMatrix[k, l] = matrix[i, j];
                    extendedMatrix.SetSubmatrix(subMatrix, i, j);
                }
            return extendedMatrix;
        }
    }
}