using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using JPEG.CommandLineParse;
using JPEG.JpegCompress;
using JPEG.JpegDecompress;
using JPEG.Modules;
using Ninject;

namespace JPEG
{
    public class Program
    {
        private static bool TryArgsParse(string[] args, out ApplicationOptions options)
        {
            var startKernel = new StandardKernel(new StartModule());
            var commandLineParser = (ICommandLineParser<ApplicationOptions>)startKernel.Get(typeof(ICommandLineParser<>));
            var parsedResult = commandLineParser.Parse(args);
            options = commandLineParser.GetResultObject();
            if (parsedResult.HasErrors)
                commandLineParser.ShowHelp();
            return !parsedResult.HasErrors;
        }

        private static int PercentToCountFrequence(int percent, int dct) =>
            (int)((double)dct * dct / 100 * percent);

        private static void SetMaxThread(int maxNumberThreads)
        {
            int maxInOutThreads;
            int maxWorkThreads;
            ThreadPool.GetMaxThreads(out maxWorkThreads, out maxInOutThreads);
            ThreadPool.SetMaxThreads(maxNumberThreads, maxInOutThreads);
        }

        private static double[,] GetColorMatrixQuantification()
        {
            var matrixQuantification = new double[,]
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
            var matrixQuantification = new double[,]
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

        private static void Main(string[] args)
        {
            ApplicationOptions options;
            if (!TryArgsParse(args, out options))
                return;

            //try
            //{
            SetMaxThread(options.MaxThreads);
            const int thinIndex = 2;
            var countSaveFrequence = PercentToCountFrequence(options.PercentCompress, options.Dct);
            var applicationModule = new ApplicationModule(options.Dct, countSaveFrequence, thinIndex, GetLumiaMatrixQuantification(), GetColorMatrixQuantification());
            var kernelApplication = new StandardKernel(applicationModule);
            if (options.PathCompressFile != null)
            {
                var jpegCompressor = kernelApplication.Get<ICompressor>();
                var bmp = (Bitmap)Image.FromFile(options.PathSourceFile);
                var compressedImage = jpegCompressor.Compress(bmp);
                compressedImage.Save(options.PathCompressFile);
            }
            if (options.PathDecompressFile != null)
            {
                var jpegDecompressor = kernelApplication.Get<IDecompressor>();
                var compressedImage = CompressedImage.Load(options.PathSourceFile, options.Dct);
                var decompressedImage = jpegDecompressor.Decompress(compressedImage);
                decompressedImage.Save(options.PathDecompressFile, ImageFormat.Bmp);
            }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //}
        }
    }
}
