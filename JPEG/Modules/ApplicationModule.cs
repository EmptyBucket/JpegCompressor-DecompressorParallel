using JPEG.ArrayToMatrixBuilder;
using JPEG.BitmapBuild;
using JPEG.ChannelExtract;
using JPEG.ChannelPack;
using JPEG.DctCompress;
using JPEG.DctDecompress;
using JPEG.JpegCompress;
using JPEG.JpegDecompress;
using JPEG.MatrixExtend;
using JPEG.MatrixThin;
using JPEG.MatrixToArrayTransform;
using JPEG.PieceMatrixExtend;
using JPEG.PixelsExtract;
using JPEG.Quantification;
using JPEG.QuantificationMatrixProvide;
using Ninject.Modules;

namespace JPEG.Modules
{
    public class ApplicationModule : NinjectModule
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
            var lumiaMatrixProvider = new LumiaQuantificationMatrixProvider();
            var colorMatrixProvider = new ColorQuantificationMatrixProvider();
            Bind<ICompressor>().To<JpegCompressor>()
                .WithConstructorArgument("thinIndex", _thinIndex)
                .WithConstructorArgument("compressionLevel", _compressionLevel)
                .WithConstructorArgument("dctSize", _dctSize)
                .WithConstructorArgument("lumiaMatrixProvider", lumiaMatrixProvider)
                .WithConstructorArgument("colorMatrixProvider", colorMatrixProvider);
            Bind<IDecompressor>().To<JpegDecompressor>()
                .WithConstructorArgument("dctSize", _dctSize)
                .WithConstructorArgument("lumiaMatrixProvider", lumiaMatrixProvider)
                .WithConstructorArgument("colorMatrixProvider", colorMatrixProvider);
            Bind<IBitmapBuilder>().To<BgrBitmapBuilder>();
            Bind(typeof(IPixelsExtractor<>)).To<BgrPixelsExtractor>();
            Bind(typeof (IChannelsExtractor<,>)).To<YCbCrChannelsExtractor>();
            Bind(typeof (IChannelsPacker<,>)).To<YCbCrChannelsPacker>();
            Bind<IDctDecompressor>().To<DctDecompressor>();
            Bind<IDctCompressor>().To<DctCompressor>();
            Bind(typeof (IPieceMatrixExtender<>)).To(typeof(DuplicatePieceMatrixExtender<>));
            Bind(typeof (IMatrixThinner<>)).To(typeof (AvgMatrixThinner));
            Bind<IMatrixExtender>().To<LastValueMatrixExtender>();
            Bind<IQuantifier>().To<Quantifier>();
            Bind<IMatrixToArrayTransformer>().To<ZigZagTransformer>();
            Bind<IArrayToMatrixBuilder>().To<ZigZagBuilder>();
        }
    }
}
