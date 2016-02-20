namespace JPEG.ExtensionsMethods
{
    public static class BitsCountConverter
    {
        public static int ToCoutBytes(int countBits) => countBits / 8;
    }
}