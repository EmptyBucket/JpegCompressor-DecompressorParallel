using System;

namespace JPEG.Quantification
{
    public class Quantifier : IQuantifier
    {
        public double[,] Quantification(double[,] matrix, double[,] matrixQuantification)
        {
            var matrixHeight = matrix.GetLength(0);
            var matrixWidth = matrix.GetLength(1);
            if(matrixHeight != matrixQuantification.GetLength(0) || matrixWidth != matrixQuantification.GetLength(1))
                throw new Exception("Mismatching sizes and the quantization matrix of the matrix");
            var quantificatedMatrix = new double[matrixHeight, matrixWidth];
            for (var i = 0; i < matrixHeight; i++)
                for (var j = 0; j < matrixWidth; j++)
                    quantificatedMatrix[i,j] = matrix[i,j] / matrixQuantification[i, j];
            return quantificatedMatrix;
        }

        public double[,] UnQuantification(double[,] matrix, double[,] matrixQuantification)
        {
             var matrixHeight = matrix.GetLength(0);
            var matrixWidth = matrix.GetLength(1);
            if(matrixHeight != matrixQuantification.GetLength(0) || matrixWidth != matrixQuantification.GetLength(1))
                throw new Exception("Mismatching sizes and the quantization matrix of the matrix");
            var unQuantificatedMatrix = new double[matrixHeight, matrixWidth];
            for (var i = 0; i < matrixHeight; i++)
                for (var j = 0; j < matrixWidth; j++)
                    unQuantificatedMatrix[i,j] = matrix[i,j] * matrixQuantification[i, j];
            return unQuantificatedMatrix;
        }
    }
}