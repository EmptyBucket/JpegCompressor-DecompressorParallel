using JPEG.Pixel;

namespace JPEG.ChannelPack
{
    public interface IChannelsPacker<in T>
    {
        YCbCrPixel[,] Pack(T yCbCrChannels, int sizeX, int sizeY);
    }
}