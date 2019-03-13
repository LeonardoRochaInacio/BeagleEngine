using System;
using Beagle.Core;

namespace Beagle.Render
{
    [BeagleModule]
    class Module : Beagle.Core.IDefaultModule
    {
        public void Startup()
        {
            Console.WriteLine(GetLogPrefix());
        }

        public void Shutdown()
        {

        }

        public string GetLogPath()
        {
            return "";
        }

        public string GetLogPrefix()
        {
            return "Render_";
        }
    }
}
