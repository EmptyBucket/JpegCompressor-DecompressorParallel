using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using JPEG.ChannelExtract;
using JPEG.DctCompress;
using JPEG.MatrixThin;
using JPEG.Pixel;
using JPEG.PixelsExtract;

namespace JPEG.JpegCompress
{
    public class JpegCompressor : ICompressor
    {
        private readonly IMatrixThinner<double> _matrixThinner;
        private readonly int _thinIndex;
        private readonly int _compressionLevel;
        private readonly int _dctSize;
        private readonly IChannelsExtractor<YCbCrChannels> _channelExtractor;
        private readonly IDctCompressor _dctCompressor;
        private readonly IPixelsExtractor<RgbPixel> _pixelsExtractor;

        public JpegCompressor(IDctCompressor dctCompressor, IPixelsExtractor<RgbPixel> pixelsExtractor, IChannelsExtractor<YCbCrChannels> channelExtractor, IMatrixThinner<double> matrixThinner, int thinIndex, int compressionLevel, int dctSize)
        {
            _matrixThinner = matrixThinner; 
            _thinIndex = thinIndex;
            _compressionLevel = compressionLevel;
            _dctSize = dctSize;
            _channelExtractor = channelExtractor;
            _dctCompressor = dctCompressor;
            _pixelsExtractor = pixelsExtractor;
        }

        public CompressedImage Compress(Bitmap bmp)
        {
            if (bmp.PixelFormat != PixelFormat.Format24bppRgb)
                throw new Exception("Формат пикселей изображения не поддерживается");
            if (bmp.Width % _dctSize != 0 || bmp.Height % _dctSize != 0)
                throw new Exception($"Image width and height must be multiple of {_dctSize}");

            var pixels = _pixelsExtractor.Extract(bmp);
            var convertedPixels = MatrixRgbToYCbCrConveter.Convert(pixels);
            var channels = _channelExtractor.Extract(convertedPixels);
            var thinnedCrChannel = _matrixThinner.Thin(channels.CrChannel, _thinIndex);
            var thinnedCbChannel = _matrixThinner.Thin(channels.CbChannel, _thinIndex);

            var yDct = _dctCompressor.Compress(channels.YChannel, _dctSize, _compressionLevel);
            var cbDct = _dctCompressor.Compress(thinnedCbChannel, _dctSize, _compressionLevel);
            var crDct = _dctCompressor.Compress(thinnedCrChannel, _dctSize, _compressionLevel);

            var result = new List<double>();
            var notThinnedStep = _thinIndex * _thinIndex;
            for (int i = 0, thinI = 0; i < yDct.Length; i += notThinnedStep, thinI++)
            {
                result.AddRange(yDct.Skip(i)
                                    .Take(notThinnedStep)
                                    .Aggregate(new List<double>(),
                                    (list, doubles) => list.Concat(doubles).ToList()));
                result.AddRange(cbDct[thinI]);
                result.AddRange(crDct[thinI]);
            }

            var compressedImage = new CompressedImage
            {
                ThinIndex = _thinIndex,
                CompressionLevel = _compressionLevel,
                Frequences = result,
                Height = bmp.Height,
                Width = bmp.Width
            };
            return compressedImage;
        }
    }
}
