using System.Collections.Generic;
using System.Linq;

namespace JPEG.ExtensionsMethods
{
    public static class ArrayExtensions
    {
         public static IEnumerable<double> ShiftArrayValue(this IEnumerable<double> array, double shiftValue) =>
            array.Select(item => item + shiftValue).ToArray();

        public static IEnumerable<byte> ShiftArrayValue(this IEnumerable<byte> array, int shiftValue) =>
            array.Select(item => (byte)(item + shiftValue)).ToArray();
    }
}