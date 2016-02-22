using System;
using System.Collections.Generic;
using System.Globalization;

namespace JPEG.CompressionAlgorithms
{
    internal static class Rle<T> where T : struct, IConvertible
    {
        private static T _rleMarker;

        private static ulong _maxLength;

        public static IEnumerable<T> Encode(IEnumerable<T> data)
        {
            var enumerator = data.GetEnumerator();

            if (!enumerator.MoveNext())
                yield break;

            var firstRunValue = enumerator.Current;
            ulong runLength = 1;
            while (enumerator.MoveNext())
            {
                var currentValue = enumerator.Current;
                if (currentValue.Equals(firstRunValue))
                    runLength++;
                else
                {
                    foreach (var item in MakeRun(firstRunValue, runLength))
                        yield return item;

                    firstRunValue = currentValue;
                    runLength = 1;
                }
                if (runLength > _maxLength)
                {
                    foreach (var item in MakeRun(firstRunValue, _maxLength))
                        yield return item;
                    runLength -= _maxLength;
                }
            }
            foreach (var item in MakeRun(firstRunValue, runLength))
                yield return item;
        }

        public static IEnumerable<T> Decode(IEnumerable<T> data)
        {
            var enumerator = data.GetEnumerator();
            if (!enumerator.MoveNext())
                yield break;

            do
            {
                var value = enumerator.Current;
                if (!value.Equals(_rleMarker))
                    yield return value;
                else
                {
                    if (!enumerator.MoveNext())
                        throw new ArgumentException("The provided data is not properly encoded.");
                    if (enumerator.Current.Equals(_rleMarker))
                        yield return value;
                    else
                    {
                        var length = enumerator.Current.ToInt64(CultureInfo.InvariantCulture);
                        if (!enumerator.MoveNext())
                            throw new ArgumentException("The provided data is not properly encoded.");
                        var val = enumerator.Current;
                        for (var j = 0; j < length; ++j)
                            yield return val;
                    }
                }
            } while (enumerator.MoveNext());
        }

        private static IEnumerable<T> MakeRun(T value, ulong length)
        {
            if ((length <= 3 && !value.Equals(_rleMarker)) || length <= 1)
                for (ulong i = 0; i < length; ++i)
                {
                    if (value.Equals(_rleMarker))
                        yield return _rleMarker;
                    yield return value;
                }
            else
            {
                yield return _rleMarker;
                yield return (T) (dynamic) length;
                yield return value;
            }
        }
    }
}