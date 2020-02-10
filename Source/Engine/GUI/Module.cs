using Beagle.Core;

namespace Beagle.GUI
{
    [BeagleModule]
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
