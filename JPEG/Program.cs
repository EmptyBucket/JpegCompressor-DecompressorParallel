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

            var applicationModule = new ApplicationModule(options.Dct, countSaveFrequence, thinIndex);
            var applicationKernel = new StandardKernel(applicationModule);

            if (options.PathCompressFile != null)
            {
                var jpegCompressor = applicationKernel.Get<ICompressor>();
                var bmp = (Bitmap)Image.FromFile(options.PathSourceFile);
                var compressedImage = jpegCompressor.Compress(bmp);
                compressedImage.Save(options.PathCompressFile);
            }
            if (options.PathDecompressFile != null)
            {
                var jpegDecompressor = applicationKernel.Get<IDecompressor>();
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
