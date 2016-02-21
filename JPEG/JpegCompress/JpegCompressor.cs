using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using JPEG.ChannelExtract;
using JPEG.DctCompress;
using JPEG.MatrixExtend;
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
        private readonly IMatrixExtender _matrixExtender;

        public JpegCompressor(IDctCompressor dctCompressor, IPixelsExtractor<RgbPixel> pixelsExtractor, IChannelsExtractor<YCbCrChannels> channelExtractor, IMatrixThinner<double> matrixThinner, int thinIndex, int compressionLevel, int dctSize, IMatrixExtender matrixExtender)
        {
            _matrixThinner = matrixThinner; 
            _thinIndex = thinIndex;
            _compressionLevel = compressionLevel;
            _dctSize = dctSize;
            _matrixExtender = matrixExtender;
            _channelExtractor = channelExtractor;
            _dctCompressor = dctCompressor;
            _pixelsExtractor = pixelsExtractor;
        }

        public CompressedImage Compress(Bitmap bmp)
        {
            if (bmp.PixelFormat != PixelFormat.Format24bppRgb)
                throw new Exception($"{bmp.PixelFormat} pixel format is not supported, supported rgb24");
            var pixels = _pixelsExtractor.Extract(bmp);
            var convertedPixels = MatrixRgbToYCbCrConveter.Convert(pixels);
            var residueY = bmp.Height%(_thinIndex*_dctSize);
            var residueX = bmp.Width%(_thinIndex*_dctSize);
            var extendedPixelsMatrix = _matrixExtender.Extend(convertedPixels, residueY == 0 ? 0 : _thinIndex*_dctSize - residueY, residueX == 0 ? 0 : _thinIndex*_dctSize - residueX);

            var channels = _channelExtractor.Extract(extendedPixelsMatrix);
            var thinnedCrChannel = _matrixThinner.Thin(channels.CrChannel, _thinIndex);
            var thinnedCbChannel = _matrixThinner.Thin(channels.CbChannel, _thinIndex);

            var yDct = _dctCompressor.Compress(channels.YChannel, _dctSize, _compressionLevel);
            var cbDct = _dctCompressor.Compress(thinnedCbChannel, _dctSize, _compressionLevel);
            var crDct = _dctCompressor.Compress(thinnedCrChannel, _dctSize, _compressionLevel);

            var result = new List<double>();
            var notThinnedStep = _thinIndex * _thinIndex;
            for (int i = 0, thinI = 0; i < yDct.Length; thinI++)
            {
                for (var j = 0; j < notThinnedStep; j++)
                    result.AddRange(yDct[i++]);
                result.AddRange(cbDct[thinI]);
                result.AddRange(crDct[thinI]);
            }

            var compressedImage = new CompressedImage
            {
                ThinIndex = _thinIndex,
                CompressionLevel = _compressionLevel,
                Frequences = result,
                Height = extendedPixelsMatrix.GetLength(0),
                Width = extendedPixelsMatrix.GetLength(1)
            };
            return compressedImage;
        }
    }
}
