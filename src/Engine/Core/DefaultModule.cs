using System;
using System.Text;

namespace Beagle.Core
{
    public class DefaultModule : IDefaultModule
    {
        private readonly StringBuilder _LoggerString = new StringBuilder();

        public StringBuilder LoggerString { get { return _LoggerString; } }

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

        public string GetModuleName()
        {
            return this.GetType().Module.Name.Replace(".dll", "").Replace(".exe", "");
        }
    }
}
