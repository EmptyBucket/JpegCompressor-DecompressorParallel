using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using JPEG.ChannelExtract;
using JPEG.ChannelPack;
using JPEG.DctCompress;
using JPEG.DctDecompress;
using JPEG.Pixel;
using JPEG.PixelsExtract;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JpegTest
{
    [TestClass]
    public class RgbToYCbCrConvertTest
    {
        [TestMethod]
        public void Rgb_ConvertToYCbCrAndBack_SameResult()
        {
            var rgb = new RgbPixel(50, 150, 255);
            var yCbCr = RgbToYCbCrConverter.Convert(rgb);
            var newRgb = YCbCrToRgbConverter.Convert(yCbCr);
        }

        [TestMethod]
        public void Test()
        {
            var bmp1 = (Bitmap)Image.FromFile(@"C:\Users\FessEmpty\Downloads\parallelprogramming-shpora2016-56f562c98bbf\parallelprogramming-shpora2016-56f562c98bbf\JPEG\sample.bmp");
            var bitmapData1 = bmp1.LockBits(new Rectangle(0, 0, bmp1.Width, bmp1.Height), ImageLockMode.ReadOnly, bmp1.PixelFormat);
            const int rgpPixelSize = 3;
            var additional = bitmapData1.Stride - bitmapData1.Width*rgpPixelSize;
            var result = new RgbPixel[bmp1.Height, bmp1.Width];
            unsafe
            {
                var imagePointer = (byte*)bitmapData1.Scan0;
                for (var j = 0; j < bitmapData1.Height; j++, imagePointer += additional)
                    for (var i = 0; i < bitmapData1.Width; i++, imagePointer += rgpPixelSize)
                        result[j, i] = new RgbPixel(imagePointer[0], imagePointer[1], imagePointer[2]);
            }
            bmp1.UnlockBits(bitmapData1);

            var convert = MatrixRgbToYCbCrConveter.Convert(result);
            var back = MatrixYCbCrToRgbConverter.Convert(convert);

            var bmp2 = new Bitmap(bmp1.Width, bmp1.Height, bmp1.PixelFormat);
            var bitmapData2 = bmp2.LockBits(new Rectangle(0, 0, bmp2.Width, bmp2.Height), ImageLockMode.ReadOnly, bmp2.PixelFormat);
            unsafe
            {
                var imagePointer = (byte*)bitmapData2.Scan0;
                for (var j = 0; j < bitmapData2.Height; j++, imagePointer += additional)
                    for (var i = 0; i < bitmapData2.Width; i++, imagePointer += rgpPixelSize)
                    {
                        imagePointer[0] = back[j, i].R;
                        imagePointer[1] = back[j, i].G;
                        imagePointer[2] = back[j, i].B;
                    }
            }
            bmp2.UnlockBits(bitmapData2);
            bmp2.Save(@"C:\Users\FessEmpty\Downloads\parallelprogramming-shpora2016-56f562c98bbf\parallelprogramming-shpora2016-56f562c98bbf\JPEG\sampleTest.bmp", ImageFormat.Bmp);
        }

         private static T[][] DevideBlocks<T>(IReadOnlyCollection<T> array, int countElementToBock) => array
            .Select((s, i) => new { Value = s, Index = i })
                .GroupBy(x => x.Index / countElementToBock)
                .Select(grp => grp.Select(x => x.Value).ToArray())
                .ToArray();
    }
}
