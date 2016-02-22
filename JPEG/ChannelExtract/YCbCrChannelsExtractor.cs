using JPEG.Pixel;

namespace JPEG.ChannelExtract
{
    public class YCbCrChannelsExtractor : IChannelsExtractor<YCbCrChannels, YCbCrPixel>
    {
        public YCbCrChannels Extract(YCbCrPixel[,] matrixPixels)
        {
            var lengthY = matrixPixels.GetLength(0);
            var lengthX = matrixPixels.GetLength(1);
            var crChannel = new double[lengthY,lengthX];
            var cbChannel = new double[lengthY,lengthX];
            var yChannel = new double[lengthY,lengthX];
            for (var i = 0; i < lengthY; i++)
                for (var j = 0; j < lengthX; j++)
                {
                    crChannel[i, j] = matrixPixels[i, j].Cr;
                    cbChannel[i, j] = matrixPixels[i, j].Cb;
                    yChannel[i, j] = matrixPixels[i, j].Y;
                }
            var channels = new YCbCrChannels(yChannel, cbChannel, crChannel);
            return channels;
        }
    }
}