using Beagle.Core;
using static CSGL.Glfw3;
using static CSGL.CSGL;

namespace Beagle.Application
{
    [BeagleModule]
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
