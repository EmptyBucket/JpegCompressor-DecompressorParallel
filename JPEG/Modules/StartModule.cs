using JPEG.CommandLineParse;

namespace JPEG.Modules
{
    public class StartModule : Ninject.Modules.NinjectModule
    {
        public override void Load() => Bind(typeof (ICommandLineParser<>)).To<ApplicationCommandLineParser>();
    }
}
