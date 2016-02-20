namespace JPEG.CommandLineParse
{
    public class ApplicationOptions
    {
        public string PathSourceFile { get; set; }
        public string PathCompressFile { get; set; }
        public int MaxThreads { get; set; }
        public int PercentCompress { get; set; }
        public string PathDecompressFile { get; set; }
        public int Dct { get; set; }
    }
}