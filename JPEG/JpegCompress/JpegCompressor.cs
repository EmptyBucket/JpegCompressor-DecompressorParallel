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
        private readonly double[,] _colorMatrixQuantification;
        private readonly double[,] _lumiaMatrixQuantification;

        public JpegCompressor(IDctCompressor dctCompressor, IPixelsExtractor<RgbPixel> pixelsExtractor, IChannelsExtractor<YCbCrChannels> channelExtractor, IMatrixThinner<double> matrixThinner, int thinIndex, int compressionLevel, int dctSize, IMatrixExtender matrixExtender, double[,] lumiaMatrixQuantification, double[,] colorMatrixQuantification)
        {
            _matrixThinner = matrixThinner; 
            _thinIndex = thinIndex;
            _compressionLevel = compressionLevel;
            _dctSize = dctSize;
            _matrixExtender = matrixExtender;
            _colorMatrixQuantification = colorMatrixQuantification;
            _lumiaMatrixQuantification = lumiaMatrixQuantification;
            _channelExtractor = channelExtractor;
            _dctCompressor = dctCompressor;
            _pixelsExtractor = pixelsExtractor;
        }

        public CompressedImage Compress(Bitmap bmp)
        {
            if (bmp.PixelFormat != PixelFormat.Format24bppRgb)
                throw new Exception($"{bmp.PixelFormat} pixel format is not supported, supported rgb24");
            var pixelsMatrix = _pixelsExtractor.Extract(bmp);
            var converterPixelsMatrix = MatrixRgbToYCbCrConveter.Convert(pixelsMatrix);
            var residueYPiece = bmp.Height%(_thinIndex*_dctSize);
            var residueXPiece = bmp.Width%(_thinIndex*_dctSize);
            var countAddYPiece = residueYPiece == 0 ? 0 : _thinIndex*_dctSize - residueYPiece;
            var countAddXPiece = residueXPiece == 0 ? 0 : _thinIndex*_dctSize - residueXPiece;
            var extendedPixelsMatrix = _matrixExtender.Extend(converterPixelsMatrix, countAddYPiece, countAddXPiece);

            var yCbCrchannels = _channelExtractor.Extract(extendedPixelsMatrix);
            var thinnedCbChannel = _matrixThinner.Thin(yCbCrchannels.CbChannel, _thinIndex);
            var thinnedCrChannel = _matrixThinner.Thin(yCbCrchannels.CrChannel, _thinIndex);

            var dctYPieces = _dctCompressor.Compress(yCbCrchannels.YChannel, _dctSize, _compressionLevel, _lumiaMatrixQuantification);
            var dctCbPieces = _dctCompressor.Compress(thinnedCbChannel, _dctSize, _compressionLevel, _colorMatrixQuantification);
            var dctCrPieces = _dctCompressor.Compress(thinnedCrChannel, _dctSize, _compressionLevel, _colorMatrixQuantification);

            var result = new List<double>();
            var countYBlocksPerColorBlock = _thinIndex * _thinIndex;
            for (int i = 0, thinI = 0; i < dctYPieces.Length; thinI++)
            {
                for (var j = 0; j < countYBlocksPerColorBlock; j++)
                    result.AddRange(dctYPieces[i++]);
                result.AddRange(dctCbPieces[thinI]);
                result.AddRange(dctCrPieces[thinI]);
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
