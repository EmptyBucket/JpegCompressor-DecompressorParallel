using Fclp;

namespace JPEG.CommandLineParse
{
    public interface ICommandLineParser<out T>
    {
        ICommandLineParserResult Parse(string[] args);

        void ShowHelp();

        T GetResultObject();
    }
}