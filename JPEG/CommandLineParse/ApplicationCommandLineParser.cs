using System;
using Fclp;

namespace JPEG.CommandLineParse
{
    public class ApplicationCommandLineParser : ICommandLineParser<ApplicationOptions>
    {
        private readonly FluentCommandLineParser<ApplicationOptions> _commandLineParser;

        public ApplicationCommandLineParser()
        {
            _commandLineParser = new FluentCommandLineParser<ApplicationOptions>();
            _commandLineParser.Setup(arg => arg.PathSourceFile)
                .As('f', "filename")
                .WithDescription("Исходный файл")
                .Required();
            _commandLineParser.Setup(arg => arg.PathCompressFile)
                .As('e', "encode")
                .WithDescription("Сжимать файл. Путь к результирующему файлу.");
            _commandLineParser.Setup(arg => arg.PercentCompress)
                .As('p', "percent")
                .WithDescription(
                    "Процент неигнорируемых старших частот. Значение по умолчанию – 100 (никакие частоты не игнорируются).")
                .SetDefault(100);
            _commandLineParser.Setup(arg => arg.MaxThreads)
                .As('t', "threads")
                .WithDescription(
                    "Максимальное кол-во потоков обработки (степень параллелизма). Значение по умолчанию – 1")
                .SetDefault(1);
            _commandLineParser.Setup(arg => arg.PathDecompressFile)
                .As('d', "decompress")
                .WithDescription(
                    "Разжимать файл. Формат распакованного файла – 24-битный bmp. Путь к результирующему файлу.");
            _commandLineParser.Setup(arg => arg.Dct)
                .As('w', "dct")
                .WithDescription("Размер окна DCT-преобразования. Значение по умолчанию – 8.")
                .SetDefault(8);
            _commandLineParser.SetupHelp()
                .Callback(text => Console.WriteLine(text));
        }

        public ApplicationOptions GetResultObject() => _commandLineParser.Object;

        public void ShowHelp() => _commandLineParser.HelpOption.ShowHelp(_commandLineParser.Options);

        public ICommandLineParserResult Parse(string[] args) => _commandLineParser.Parse(args);
    }
}
