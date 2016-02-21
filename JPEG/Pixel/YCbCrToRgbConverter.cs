namespace JPEG.Pixel
{
    public static class YCbCrToRgbConverter
    {
        private static byte DoubleToByte(double d)
        {
            var intD = (int) d;
            var byteD = intD > byte.MaxValue
                ? byte.MaxValue : intD < byte.MinValue
                ? byte.MinValue : (byte) intD;
            return byteD;
        }

        public static RgbPixel Convert(YCbCrPixel yCbCrPixel)
        {
            var rChannel = DoubleToByte(yCbCrPixel.Y + 1.402525 * yCbCrPixel.Cr);
            var gChannel = DoubleToByte(yCbCrPixel.Y - 0.343730 * yCbCrPixel.Cb - 0.71440 * yCbCrPixel.Cr);
            var bChannel = DoubleToByte(yCbCrPixel.Y + 1.769905 * yCbCrPixel.Cb);

            var rgb = new RgbPixel(rChannel, gChannel, bChannel);

            return rgb;
        }
    }
}