namespace JPEG.Pixel
{
    public static class RgbToYCbCrConverter
    {
        public static YCbCrPixel Convert(RgbPixel rgb)
        {
            var yChannel = 0.299*rgb.R + 0.587*rgb.G + 0.114*rgb.B;
            var cbChannel = -0.16874*rgb.R - 0.33126*rgb.G + 0.5*rgb.B;
            var crChannel = 0.5*rgb.R - 0.41869*rgb.G - 0.08131*rgb.B;

            var yCbCr = new YCbCrPixel(yChannel, cbChannel, crChannel);

            return yCbCr;
        }
    }
}