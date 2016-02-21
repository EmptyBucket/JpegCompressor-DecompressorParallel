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
            var rgb = new RgbPixel(41, 71, 101);
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

        [TestMethod]
        public void Test1()
        {
            var bmp = (Bitmap)Image.FromFile(@"C:\Users\FessEmpty\Downloads\parallelprogramming-shpora2016-56f562c98bbf\parallelprogramming-shpora2016-56f562c98bbf\JPEG\1.bmp");
            var width = bmp.Width;
            var height = bmp.Height;
            var dctCompressor = new DctCompressor();
            var pixelsExtractor = new BgrPixelsExtractor();
            var channelExtractor = new YCbCrChannelsExtractor();
            const int dctSize = 8;
            const int thinIndex = 1;
            const int compressionLevel = 12;
            var pixels = pixelsExtractor.Extract(bmp);
            var convertedPixels = MatrixRgbToYCbCrConveter.Convert(pixels);
            var channels = channelExtractor.Extract(convertedPixels);
            var yDct = dctCompressor.Compress(channels.YChannel, dctSize, compressionLevel);
            var cbDct = dctCompressor.Compress(channels.CbChannel, dctSize, compressionLevel);
            var crDct = dctCompressor.Compress(channels.CrChannel, dctSize, compressionLevel);
            var result = new List<double>();
            const int notThinnedStep = thinIndex * thinIndex;
            for (int i = 0, thinI = 0; i < yDct.Length; thinI++)
            {
                for (var j = 0; j < notThinnedStep; j++)
                    result.AddRange(yDct[i++]);
                result.AddRange(cbDct[thinI]);
                result.AddRange(crDct[thinI]);
            }

            var dctDecompressor = new DctDecompressor();
            var channelPacker = new YCbCrChannelsPacker();
            var yDctDecompress = new List<double>();
            var cbDctDecompress = new List<double>();
            var crDctDecompress = new List<double>();
            const int countYBlocks = thinIndex*thinIndex;
            const int cellsToBlock = compressionLevel;
            for (var i = 0; i < result.Count;)
            {
                for (var j = 0; j < countYBlocks*cellsToBlock; j++)
                    yDctDecompress.Add(result[i++]);
                for (var j = 0; j < cellsToBlock; j++)
                    cbDctDecompress.Add(result[i++]);
                for (var j = 0; j < cellsToBlock; j++)
                    crDctDecompress.Add(result[i++]);
            }
            var yDctBlocks = DevideBlocks(yDctDecompress, compressionLevel);
            var cbDctBlocks = DevideBlocks(cbDctDecompress, compressionLevel);
            var crDctBlocks = DevideBlocks(crDctDecompress, compressionLevel);
            var yChannel = dctDecompressor.Decompress(yDctBlocks, dctSize, height, width);
            var cbChannel = dctDecompressor.Decompress(cbDctBlocks, dctSize, height / thinIndex, width / thinIndex);
            var crChannel = dctDecompressor.Decompress(crDctBlocks, dctSize, height / thinIndex, width / thinIndex);
            var yCbCrChannels = new YCbCrChannels(yChannel, cbChannel, crChannel);
            var yCbCrPixels = channelPacker.Pack(yCbCrChannels, height, width);
            var rgbPixels = MatrixYCbCrToRgbConverter.Convert(yCbCrPixels);
        }
    }
}
