namespace JPEG.ExtensionsMethods
{
    public class DoubleToByteConverter
    {
        public static byte Convert(double d)
        {
            var intD = (int) d;
            var byteD = intD > byte.MaxValue
                ? byte.MaxValue : intD < byte.MinValue
                    ? byte.MinValue : (byte) intD;
            return byteD;
        }
    }
}