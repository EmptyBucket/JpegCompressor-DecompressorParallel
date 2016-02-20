namespace JPEG.Pixel
{
    public static class YCbCrToRgbConverter
    {
        private static byte DoubleToByte(double d)
        {
            var intD = (int) d;
            return intD > byte.MaxValue
                ? byte.MaxValue : intD < byte.MinValue
                ? byte.MinValue : (byte) intD;
        }

        public static RgbPixel Convert(YCbCrPixel yCbCrPixel)
        {
            var rChannel = DoubleToByte(yCbCrPixel.Y + 1.402*yCbCrPixel.Cr);
            var gChannel = DoubleToByte(yCbCrPixel.Y - 0.34414 * yCbCrPixel.Cb - 0.71414*yCbCrPixel.Cr);
            var bChannel = DoubleToByte(yCbCrPixel.Y + 1.772*yCbCrPixel.Cb);

            var rgb = new RgbPixel(rChannel, bChannel, gChannel);

            return rgb;
        }
    }
}