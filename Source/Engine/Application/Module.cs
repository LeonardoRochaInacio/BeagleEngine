using Beagle.Core;

namespace Beagle.Application
{
    [BeagleModule("Application")]
    class Module : DefaultModule
    {
        override public void Startup()
        {

        }

        override public string GetLogPath()
        {
            return "";
        }

        override public string GetLogPrefix()
        {
            return "Application";
        }
    }
}
