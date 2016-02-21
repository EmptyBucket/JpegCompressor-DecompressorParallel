namespace JPEG.Pixel
{
    public static class RgbToYCbCrConverter
    {
        public static YCbCrPixel Convert(RgbPixel rgb)
        {
            var yChannel = 0.299 * rgb.R + 0.587 * rgb.G + 0.114 * rgb.B;
            var cbChannel = -0.168935 * rgb.R - 0.331665 * rgb.G + 0.50059 * rgb.B;
            var crChannel = 0.499813 * rgb.R - 0.418531 * rgb.G - 0.081282 * rgb.B;

            var yCbCr = new YCbCrPixel(yChannel, cbChannel, crChannel);

            return yCbCr;
        }
    }
}