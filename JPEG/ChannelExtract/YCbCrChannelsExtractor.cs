using JPEG.Pixel;

namespace JPEG.ChannelExtract
{
    public class YCbCrChannelsExtractor : IChannelsExtractor<YCbCrChannels>
    {
        public YCbCrChannels Extract(YCbCrPixel[,] pixels)
        {
            var lengthY = pixels.GetLength(0);
            var lengthX = pixels.GetLength(1);
            var crChannel = new double[lengthY,lengthX];
            var cbChannel = new double[lengthY,lengthX];
            var yChannel = new double[lengthY,lengthX];
            for (var i = 0; i < lengthY; i++)
                for (var j = 0; j < lengthX; j++)
                {
                    crChannel[i, j] = pixels[i, j].Cr;
                    cbChannel[i, j] = pixels[i, j].Cb;
                    yChannel[i, j] = pixels[i, j].Y;
                }
            var channels = new YCbCrChannels(yChannel, cbChannel, crChannel);
            return channels;
        }
    }
}