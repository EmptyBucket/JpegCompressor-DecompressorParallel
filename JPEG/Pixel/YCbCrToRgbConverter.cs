using JPEG.ExtensionsMethods;

namespace JPEG.Pixel
{
    public static class YCbCrToRgbConverter
    {
        public static RgbPixel Convert(YCbCrPixel yCbCrPixel)
        {
            var rChannel = DoubleToByteConverter.Convert(yCbCrPixel.Y + 1.402525 * yCbCrPixel.Cr);
            var gChannel = DoubleToByteConverter.Convert(yCbCrPixel.Y - 0.343730 * yCbCrPixel.Cb - 0.71440 * yCbCrPixel.Cr);
            var bChannel = DoubleToByteConverter.Convert(yCbCrPixel.Y + 1.769905 * yCbCrPixel.Cb);

            var rgb = new RgbPixel(rChannel, gChannel, bChannel);

            return rgb;
        }
    }
}