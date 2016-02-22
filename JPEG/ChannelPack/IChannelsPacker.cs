namespace JPEG.ChannelPack
{
    public interface IChannelsPacker<in TChannelsType, out TPixelType>
    {
        TPixelType[,] Pack(TChannelsType yCbCrChannels, int sizeX, int sizeY);
    }
}