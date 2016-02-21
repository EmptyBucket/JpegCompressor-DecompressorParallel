﻿using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using JPEG.BitmapBuild;
using JPEG.ChannelExtract;
using JPEG.ChannelPack;
using JPEG.DctDecompress;
using JPEG.MatrixExtend;
using JPEG.Pixel;

namespace JPEG.JpegDecompress
{
    public class JpegDecompressor : IDecompressor
    {
        private readonly IDctDecompressor _dctDecompressor;
        private readonly int _dctSize;
        private readonly IPieceMatrixExtender<double> _pieceMatrixExtender;
        private readonly IChannelsPacker<YCbCrChannels> _iChannelsPacker;
        private readonly IBitmapBuilder _bitmapBuilder;

        public  JpegDecompressor(IDctDecompressor dctDecompressor, IBitmapBuilder bitmapBuilder, IChannelsPacker<YCbCrChannels> iChannelsPacker, IPieceMatrixExtender<double> pieceMatrixExtender, int dctSize)
        {
            _dctDecompressor = dctDecompressor;
            _dctSize = dctSize;
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
            var yDct = new List<double>();
            var cbDct = new List<double>();
            var crDct = new List<double>();
            var countYBlocks = compressedImage.ThinIndex*compressedImage.ThinIndex;
            var cellsToBlock = compressedImage.CompressionLevel;
            for (var i = 0; i < compressedImage.Frequences.Count;)
            {
                for (var j = 0; j < countYBlocks*cellsToBlock; j++)
                    yDct.Add(compressedImage.Frequences[i++]);
                for (var j = 0; j < cellsToBlock; j++)
                    cbDct.Add(compressedImage.Frequences[i++]);
                for (var j = 0; j < cellsToBlock; j++)
                    crDct.Add(compressedImage.Frequences[i++]);
            }

            var yDctBlocks = DevideIntoPiece(yDct, compressedImage.CompressionLevel);
            var cbDctBlocks = DevideIntoPiece(cbDct, compressedImage.CompressionLevel);
            var crDctBlocks = DevideIntoPiece(crDct, compressedImage.CompressionLevel);
            var yChannel = _dctDecompressor.Decompress(yDctBlocks, _dctSize, compressedImage.Height, compressedImage.Width);
            var thinnedCbChannel = _dctDecompressor.Decompress(cbDctBlocks, _dctSize, compressedImage.Height / compressedImage.ThinIndex, compressedImage.Width / compressedImage.ThinIndex);
            var thinnedCrChannel = _dctDecompressor.Decompress(crDctBlocks, _dctSize, compressedImage.Height / compressedImage.ThinIndex, compressedImage.Width / compressedImage.ThinIndex);
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
