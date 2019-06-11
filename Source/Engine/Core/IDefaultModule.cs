
namespace Beagle.Core
{
    /// <summary>
    /// Interface for DefaultModule
    /// </summary>
    public interface IDefaultModule
    {
        /// <summary>
        /// Startup module method
        /// </summary>
        void Startup();

        /// <summary>
        /// Gets a module log path
        /// </summary>
        /// <returns></returns>
        string GetLogPath();

        /// <summary>
        /// Gets a module log prefix
        /// </summary>
        /// <returns></returns>
        string GetLogPrefix();
    }
}
