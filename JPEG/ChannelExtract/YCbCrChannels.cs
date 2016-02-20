namespace JPEG.ChannelExtract
{
    public class YCbCrChannels
    {
        public YCbCrChannels(double[,] yChannel, double[,] crChannel, double[,] cbChannel)
        {
            YChannel = yChannel;
            CrChannel = crChannel;
            CbChannel = cbChannel;
        }

        public double[,] YChannel { get; }
        public double[,] CrChannel { get; }
        public  double[,] CbChannel { get; }
    }
}