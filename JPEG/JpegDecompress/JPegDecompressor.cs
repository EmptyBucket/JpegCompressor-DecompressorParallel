using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using JPEG.BitmapBuild;
using JPEG.ChannelExtract;
using JPEG.ChannelPack;
using JPEG.CompressionAlgorithms;
using JPEG.DctDecompress;
using JPEG.MatrixExtend;
using JPEG.Pixel;

namespace JPEG.JpegDecompress
{
    public class JpegDecompressor : IDecompressor
    {
        private readonly IDctDecompressor _dctDecompressor;
        private readonly int _dctSize;
        private readonly double[,] _lumiaMatrixQuantification;
        private readonly double[,] _colorMatrixQuantification;
        private readonly IPieceMatrixExtender<double> _pieceMatrixExtender;
        private readonly IChannelsPacker<YCbCrChannels> _iChannelsPacker;
        private readonly IBitmapBuilder _bitmapBuilder;

        public  JpegDecompressor(IDctDecompressor dctDecompressor, IBitmapBuilder bitmapBuilder, IChannelsPacker<YCbCrChannels> iChannelsPacker, IPieceMatrixExtender<double> pieceMatrixExtender, int dctSize, double[,] lumiaMatrixQuantification, double[,] colorMatrixQuantification)
        {
            _dctDecompressor = dctDecompressor;
            _dctSize = dctSize;
            _lumiaMatrixQuantification = lumiaMatrixQuantification;
            _colorMatrixQuantification = colorMatrixQuantification;
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
            var unRle = RLE<byte>.Decode(unHuf).ToArray();

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
            var yChannel = _dctDecompressor.Decompress(yDctBlocks, _dctSize, compressedImage.Height, compressedImage.Width, _lumiaMatrixQuantification);
            var thinnedCbChannel = _dctDecompressor.Decompress(cbDctBlocks, _dctSize, compressedImage.Height / compressedImage.ThinIndex, compressedImage.Width / compressedImage.ThinIndex, _colorMatrixQuantification);
            var thinnedCrChannel = _dctDecompressor.Decompress(crDctBlocks, _dctSize, compressedImage.Height / compressedImage.ThinIndex, compressedImage.Width / compressedImage.ThinIndex, _colorMatrixQuantification);
            var cbChannel = _pieceMatrixExtender.Extend(thinnedCbChannel, compressedImage.ThinIndex);
            var crChannel = _pieceMatrixExtender.Extend(thinnedCrChannel, compressedImage.ThinIndex);
            var yCbCrChannels = new YCbCrChannels(yChannel, cbChannel, crChannel);
            var yCbCrPixels = _iChannelsPacker.Pack(yCbCrChannels, compressedImage.Height, compressedImage.Width);
            var rgbPixels = MatrixYCbCrToRgbConverter.Convert(yCbCrPixels);
            var bmp = _bitmapBuilder.Build(rgbPixels, PixelFormat.Format24bppRgb);
            return bmp;
        }
    }
}
