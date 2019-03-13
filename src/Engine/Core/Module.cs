using System;

namespace Beagle.Core
{
    [BeagleModule]
    class Module : IDefaultModule
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
            return "Core_";
        }
    }
}
