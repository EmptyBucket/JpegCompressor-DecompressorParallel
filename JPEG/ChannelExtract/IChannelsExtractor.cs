namespace JPEG.ChannelExtract
{
    public interface IChannelsExtractor<out TChannelsType, in TPixelType>
    {
        TChannelsType Extract(TPixelType[,] matrixPixels);
    }
}