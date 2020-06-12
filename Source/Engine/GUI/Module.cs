using Beagle.Core;

namespace Beagle.GUI
{
    [BeagleModule("GUI")]
    class Module : Beagle.Core.DefaultModule
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
            return "GUI";
        }
    }
}
