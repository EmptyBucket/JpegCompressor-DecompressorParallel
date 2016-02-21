using JPEG.ExtensionsMethods;

namespace JPEG.MatrixExtend
{
    public class LastValueMatrixExtender : IMatrixExtender
    {
        public T[,] Extend<T>(T[,] matrix, int yExtend, int xExtend)
        {
            var height = matrix.GetLength(0);
            var width = matrix.GetLength(1);
            var extendedHeight = height + yExtend;
            var extendedWidth = width + xExtend;
            var extendedMatrix = new T[extendedHeight, extendedWidth];
            extendedMatrix.SetSubmatrix(matrix, 0, 0);
            if (yExtend == 0 && xExtend == 0)
                return extendedMatrix;
            for (var i = 0; i < extendedHeight; i++)
                for (var j = width; j < extendedWidth; j++)
                    extendedMatrix[i, j] = extendedMatrix[i, width - 1];
            for (var i = 0; i < extendedWidth; i++)
                for (var j = height; j < extendedHeight; j++)
                    extendedMatrix[j, i] = extendedMatrix[height - 1, i];
            return extendedMatrix;
        }
    }
}