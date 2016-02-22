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
using JPEG.Quantification;
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

        private static double[,] GetColorMatrixQuantification()
        {
            var matrixQuantification = new double[8, 8]
            {
                {17,18,24,47,99,99,99,99},
                {18,21,26,66,99,99,99,99},
                {24,26,56,99,99,99,99,99},
                {47,66,99,99,99,99,99,99},
                {99,99,99,99,99,99,99,99},
                {99,99,99,99,99,99,99,99},
                {99,99,99,99,99,99,99,99},
                {99,99,99,99,99,99,99,99}
            };
            return matrixQuantification;
        }

        private static double[,] GetLumiaMatrixQuantification()
        {
            var matrixQuantification = new double[8, 8]
            {
                {16,11,10,16,24,40,51,61},
                {12,12,14,19,26,58,60,55},
                {14,13,16,24,40,57,69,56},
                {14,17,22,29,51,87,80,62},
                {18,22,37,56,68,109,103,77},
                {24,35,55,64,81,104,113,92},
                {49,64,78,87,103,121,120,101},
                {72,92,95,98,112,100,103,99}
            };
            return matrixQuantification;
        }

        public override void Load()
        {
            Bind<ICompressor>().To<JpegCompressor>()
                .WithConstructorArgument("thinIndex", _thinIndex)
                .WithConstructorArgument("compressionLevel", _compressionLevel)
                .WithConstructorArgument("dctSize", _dctSize)
                .WithConstructorArgument("lumiaMatrixQuantification", GetLumiaMatrixQuantification())
                .WithConstructorArgument("colorMatrixQuantification", GetColorMatrixQuantification());
            Bind<IDecompressor>().To<JpegDecompressor>()
                .WithConstructorArgument("dctSize", _dctSize)
                .WithConstructorArgument("lumiaMatrixQuantification", GetLumiaMatrixQuantification())
                .WithConstructorArgument("colorMatrixQuantification", GetColorMatrixQuantification());
            Bind<IBitmapBuilder>().To<BitmapBuilder>();
            Bind(typeof(IPixelsExtractor<>)).To<BgrPixelsExtractor>();
            Bind(typeof (IChannelsExtractor<>)).To<YCbCrChannelsExtractor>();
            Bind(typeof (IChannelsPacker<>)).To<YCbCrChannelsPacker>();
            Bind<IDctDecompressor>().To<DctDecompressor>();
            Bind<IDctCompressor>().To<DctCompressor>();
            Bind(typeof (IPieceMatrixExtender<>)).To(typeof(DuplicatePieceMatrixExtender<>));
            Bind(typeof (IMatrixThinner<>)).To(typeof (AvgMatrixThinner));
            Bind<IMatrixExtender>().To<LastValueMatrixExtender>();
            Bind<IQuantifier>().To<Quantifier>();
        }
    }
}
