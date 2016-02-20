namespace JPEG.Pixel
{
    public static class MatrixYCbCrToRgbConverter
    {
        public static RgbPixel[,] Convert(YCbCrPixel[,] matrix)
        {
            var height = matrix.GetLength(0);
            var width = matrix.GetLength(1);
            var convertedMatrix = new RgbPixel[height, width];
            for (var i = 0; i < height; i++)
                for (var j = 0; j < width; j++)
                    convertedMatrix[i, j] = YCbCrToRgbConverter.Convert(matrix[i, j]);
            return convertedMatrix;
        }
    }
}