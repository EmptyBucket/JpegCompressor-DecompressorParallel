using System;
using JPEG.ChannelExtract;
using JPEG.Pixel;

namespace JPEG.ChannelPack
{
    public class YCbCrChannelsPacker : IChannelsPacker<YCbCrChannels, YCbCrPixel>
    {
        public YCbCrPixel[,] Pack(YCbCrChannels yCbCrChannels, int height, int width)
        {
            var sizeY = yCbCrChannels.YChannel.GetLength(0);
            var sizeX = yCbCrChannels.YChannel.GetLength(1);
            if(height != sizeY || width!= sizeX)
                throw new Exception("The number of cells in the channels do not coincide with the number of cells in an image");
           var matrixYCbCrPixels = new YCbCrPixel[sizeY, sizeX];
            for (var i = 0; i < sizeY; i++)
                for (var j = 0; j < sizeX; j++)
                    matrixYCbCrPixels[i, j] = new YCbCrPixel(yCbCrChannels.YChannel[i, j], yCbCrChannels.CbChannel[i, j],
                        yCbCrChannels.CrChannel[i, j]);
            return matrixYCbCrPixels;
        }
    }
}