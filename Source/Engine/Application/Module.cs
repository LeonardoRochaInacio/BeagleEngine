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
            Log.Info("Trying load and init Glfw");
            csglLoadGlfw();
            if (glfwInit() <= 0)
            {
                Log.Error("Error on glfwInit()");
                return;
            }
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
