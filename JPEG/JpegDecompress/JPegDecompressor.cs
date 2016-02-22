using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using JPEG.BitmapBuild;
using JPEG.ChannelExtract;
using JPEG.ChannelPack;
using JPEG.CompressionAlgorithms;
using JPEG.DctDecompress;
using JPEG.PieceMatrixExtend;
using JPEG.Pixel;
using JPEG.QuantificationMatrixProvide;

namespace JPEG.JpegDecompress
{
    public class JpegDecompressor : IDecompressor
    {
        private readonly IDctDecompressor _dctDecompressor;
        private readonly int _dctSize;
        private readonly IPieceMatrixExtender<double> _pieceMatrixExtender;
        private readonly IChannelsPacker<YCbCrChannels, YCbCrPixel> _iChannelsPacker;
        private readonly IBitmapBuilder _bitmapBuilder;
        private readonly IMatrixProvider<double> _lumiaMatrixProvider;
        private readonly IMatrixProvider<double> _colorMatrixProvider;

        public  JpegDecompressor(IDctDecompressor dctDecompressor, IBitmapBuilder bitmapBuilder, IChannelsPacker<YCbCrChannels, YCbCrPixel> iChannelsPacker, IPieceMatrixExtender<double> pieceMatrixExtender, int dctSize, IMatrixProvider<double> lumiaMatrixProvider, IMatrixProvider<double> colorMatrixProvider)
        {
            _dctDecompressor = dctDecompressor;
            _dctSize = dctSize;
            _lumiaMatrixProvider = lumiaMatrixProvider;
            _colorMatrixProvider = colorMatrixProvider;
            _pieceMatrixExtender = pieceMatrixExtender;
            _iChannelsPacker = iChannelsPacker;
            _bitmapBuilder = bitmapBuilder;
        }

        private static T[][] DevideIntoPiece<T>(IReadOnlyCollection<T> array, int countElementToBock) => array
            .Select((s, i) => new { Value = s, Index = i })
                .GroupBy(x => x.Index / countElementToBock)
                .Select(grp => grp.Select(x => x.Value).ToArray())
                .ToArray();

        public Bitmap Decompress(CompressedImage compressedImage)
        {
            var unHuf = HuffmanCodec.Decode(compressedImage.DataBytes, compressedImage.DecodeTable,
                compressedImage.BitsCount);
            var unRle = Rle<byte>.Decode(unHuf).ToArray();

            var yDct = new List<double>();
            var cbDct = new List<double>();
            var crDct = new List<double>();
            var countYBlocks = compressedImage.ThinIndex*compressedImage.ThinIndex;
            var cellsToBlock = compressedImage.CompressionLevel;
            for (var i = 0; i < unRle.Length;)
            {
                for (var j = 0; j < countYBlocks*cellsToBlock; j++)
                    yDct.Add(unRle[i++]);
                for (var j = 0; j < cellsToBlock; j++)
                    cbDct.Add(unRle[i++]);
                for (var j = 0; j < cellsToBlock; j++)
                    crDct.Add(unRle[i++]);
            }

            var yDctBlocks = DevideIntoPiece(yDct, compressedImage.CompressionLevel);
            var cbDctBlocks = DevideIntoPiece(cbDct, compressedImage.CompressionLevel);
            var crDctBlocks = DevideIntoPiece(crDct, compressedImage.CompressionLevel);
            var yChannel = _dctDecompressor.Decompress(yDctBlocks, _dctSize, compressedImage.Height, compressedImage.Width, _lumiaMatrixProvider.GetMatrix());
            var thinnedCbChannel = _dctDecompressor.Decompress(cbDctBlocks, _dctSize, compressedImage.Height / compressedImage.ThinIndex, compressedImage.Width / compressedImage.ThinIndex, _colorMatrixProvider.GetMatrix());
            var thinnedCrChannel = _dctDecompressor.Decompress(crDctBlocks, _dctSize, compressedImage.Height / compressedImage.ThinIndex, compressedImage.Width / compressedImage.ThinIndex, _colorMatrixProvider.GetMatrix());
            var cbChannel = _pieceMatrixExtender.Extend(thinnedCbChannel, compressedImage.ThinIndex);
            var crChannel = _pieceMatrixExtender.Extend(thinnedCrChannel, compressedImage.ThinIndex);
            var yCbCrChannels = new YCbCrChannels(yChannel, cbChannel, crChannel);
            var yCbCrPixels = _iChannelsPacker.Pack(yCbCrChannels, compressedImage.Height, compressedImage.Width);
            var matrixRgbPixels = MatrixYCbCrToRgbConverter.Convert(yCbCrPixels);
            var bmp = _bitmapBuilder.Build(matrixRgbPixels);
            return bmp;
        }
    }
}
