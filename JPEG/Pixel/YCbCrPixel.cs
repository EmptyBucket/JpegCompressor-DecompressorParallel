namespace JPEG.Pixel
{
    public class YCbCrPixel
    {
        public YCbCrPixel(double y, double cb, double cr)
        {
            Y = y;
            Cb = cb;
            Cr = cr;
        }

        public double Y { get; }
        public double Cr { get; }
        public double Cb { get; }
    }
}