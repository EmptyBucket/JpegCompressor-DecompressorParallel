using JPEG.BitmapBuild;
using JPEG.ChannelExtract;
using JPEG.ChannelPack;
using JPEG.DctCompress;
using JPEG.DctDecompress;
using JPEG.JpegCompress;
using JPEG.JpegDecompress;
using JPEG.MatrixExtend;
using JPEG.MatrixThin;
using JPEG.PixelsExtract;

namespace JPEG.Modules
{
    public class ApplicationModule : Ninject.Modules.NinjectModule
    {
        private readonly int _dctSize;
        private readonly int _thinIndex;
        private readonly int _compressionLevel;

        public ApplicationModule(int dctSize, int compressionLevel, int thinIndex)
        {
            _dctSize = dctSize;
            _thinIndex = thinIndex;
            _compressionLevel = compressionLevel;
        }

        public override void Load()
        {
            Bind<ICompressor>().To<JpegCompressor>()
                .WithConstructorArgument("thinIndex", _thinIndex)
                .WithConstructorArgument("compressionLevel", _compressionLevel)
                .WithConstructorArgument("dctSize", _dctSize);
            Bind<IDecompressor>().To<JpegDecompressor>()
                .WithConstructorArgument("dctSize", _dctSize);
            Bind<IBitmapBuilder>().To<BitmapBuilder>();
            Bind(typeof(IPixelsExtractor<>)).To<BgrPixelsExtractor>();
            Bind(typeof (IChannelsExtractor<>)).To<YCbCrChannelsExtractor>();
            Bind(typeof (IChannelsPacker<>)).To<YCbCrChannelsPacker>();
            Bind<IDctDecompressor>().To<DctDecompressor>();
            Bind<IDctCompressor>().To<DctCompressor>();
            Bind(typeof (IMatrixExtender<>)).To(typeof(DuplicateMatrixExtender<>));
            Bind(typeof (IMatrixThinner<>)).To(typeof (AvgMatrixThinner));
        }
    }
}
