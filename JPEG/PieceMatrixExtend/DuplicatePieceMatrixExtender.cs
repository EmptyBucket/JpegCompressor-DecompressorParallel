using JPEG.ExtensionsMethods;

namespace JPEG.MatrixExtend
{
    public class DuplicatePieceMatrixExtender<T> : IPieceMatrixExtender<T>
    {
        public T[,] Extend(T[,] matrix, int extendIndex)
        {
            var height = matrix.GetLength(0);
            var width = matrix.GetLength(1);
            var extendedMatrix = new T[height*extendIndex, width*extendIndex];
            for (int i = 0, extendI = 0; i < height; i++, extendI+=extendIndex)
                for (int j = 0, extendJ = 0; j < width; j++, extendJ+=extendIndex)
                {
                    var subMatrix = new T[extendIndex, extendIndex];
                    for (var k = 0; k < extendIndex; k++)
                        for (var l = 0; l < extendIndex; l++)
                            subMatrix[k, l] = matrix[i, j];
                    extendedMatrix.SetSubmatrix(subMatrix, extendI, extendJ);
                }
            return extendedMatrix;
        }
    }
}