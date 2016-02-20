namespace JPEG.Pixel
{
    public class YCbCrPixel
    {
        public YCbCrPixel(double y, double cr, double cb)
        {
            Y = y;
            Cr = cr;
            Cb = cb;
        }

        public double Y { get; }
        public double Cr { get; }
        public double Cb { get; }
    }
}