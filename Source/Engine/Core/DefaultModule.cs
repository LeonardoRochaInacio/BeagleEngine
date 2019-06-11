using System;
using System.Text;

namespace Beagle.Core
{
    /// <summary>
    /// Default module. Must be inherited on Module.cs on each module.
    /// </summary>
    public class DefaultModule : IDefaultModule
    {
        /// <summary>
        /// Abstraction, object holder of LoggetString
        /// </summary>
        private readonly StringBuilder _LoggerString = new StringBuilder();

        /// <summary>
        /// Gets a string builder of this module.
        /// </summary>
        public StringBuilder LoggerString { get { return _LoggerString; } }

        /// <summary>
        /// Implements a startup method
        /// </summary>
        virtual public void Startup()
        {
            
        }

        /// <summary>
        /// Implements a get log path method
        /// </summary>
        /// <returns></returns>
        virtual public string GetLogPath()
        {
            return "";
        }

        /// <summary>
        /// Implements a get log prefix method
        /// </summary>
        /// <returns></returns>
        virtual public string GetLogPrefix()
        {
            return "General";
        }

        /// <summary>
        /// Gets this module name.
        /// </summary>
        /// <returns></returns>
        public string GetModuleName()
        {
            return this.GetType().Module.Name.Replace(".dll", "").Replace(".exe", "");
        }
    }
}
