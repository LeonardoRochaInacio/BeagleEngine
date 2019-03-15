using System;

namespace Beagle.Core
{
    public class DefaultModule : IDefaultModule
    {
        virtual public void Startup()
        {

        }

        virtual public string GetLogPath()
        {
            return "";
        }

        virtual public string GetLogPrefix()
        {
            return "General";
        }
    }
}
