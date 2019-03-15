
namespace Beagle.Core
{
    public interface IDefaultModule
    {
        void Startup();

        string GetLogPath();

        string GetLogPrefix();
    }
}
