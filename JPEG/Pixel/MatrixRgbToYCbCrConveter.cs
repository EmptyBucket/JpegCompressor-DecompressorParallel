namespace JPEG.Pixel
{
    public static class MatrixRgbToYCbCrConveter
    {
        public static YCbCrPixel[,] Convert(RgbPixel[,] matrix)
        {
            var height = matrix.GetLength(0);
            var width = matrix.GetLength(1);
            var convertedMatrix = new YCbCrPixel[height, width];
            for (var i = 0; i < height; i++)
                for (var j = 0; j < width; j++)
                    convertedMatrix[i, j] = RgbToYCbCrConverter.Convert(matrix[i, j]);
            return convertedMatrix;
        }
    }
}