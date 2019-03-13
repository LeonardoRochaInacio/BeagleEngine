
namespace Beagle.Core
{
    public interface IDefaultModule
    {
        void Startup();

        void Shutdown();

        string GetLogPath();

        string GetLogPrefix();
    }
}
