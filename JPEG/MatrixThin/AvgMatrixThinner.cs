using System;
using JPEG.ExtensionsMethods;

namespace JPEG.MatrixThin
{
    public class AvgMatrixThinner : IMatrixThinner<double>
    {
        public double[,] Thin(double[,] array, int thinIndex)
        {
            var yLength = array.GetLength(0);
            var xLength = array.GetLength(1);
            var newYLenght = (int)Math.Ceiling((double) yLength/thinIndex);
            var newXLenght = (int)Math.Ceiling((double) xLength/thinIndex);
            var thinnedMatrix = new double[newYLenght, newXLenght];
            for (int i = 0, thinI = 0; i < yLength; i += thinIndex, thinI++)
                for (int j = 0, thinJ = 0; j < xLength; j += thinIndex, thinJ++)
                {
                    var yCount = i + thinIndex < yLength ? thinIndex : yLength - i;
                    var xCount = j + thinIndex < xLength ? thinIndex : xLength - j;
                    var subMatrix = array.GetSubMatrix(i, yCount, j, xCount);
                    double avg = 0;
                    var subMatrixYLenght = subMatrix.GetLength(0);
                    var subMatrixXLenght = subMatrix.GetLength(1);
                    for (var k = 0; k < subMatrixYLenght; k++)
                        for (var l = 0; l < subMatrixXLenght; l++)
                            avg += subMatrix[k, l];
                    avg /= subMatrixXLenght*subMatrixYLenght;
                    thinnedMatrix[thinI, thinJ] = avg;
                }
            return thinnedMatrix;
        }
    }
}
