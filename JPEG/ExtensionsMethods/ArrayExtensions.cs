using System.Linq;

namespace JPEG.ExtensionsMethods
{
    public static class ArrayExtensions
    {
         public static double[] ShiftArrayValue(this double[] array, double shiftValue) =>
            array.Select(item => item + shiftValue).ToArray();

        public static byte[] ShiftArrayValue(this byte[] array, int shiftValue) =>
            array.Select(item => (byte)(item + shiftValue)).ToArray();
    }
}