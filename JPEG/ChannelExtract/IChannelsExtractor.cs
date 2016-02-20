using JPEG.Pixel;

namespace JPEG.ChannelExtract
{
    public interface IChannelsExtractor<out T>
    {
        YCbCrChannels Extract(YCbCrPixel[,] pixels);
    }
}