using Beagle.Core;
using static CSGL.Glfw3;
using static CSGL.CSGL;

namespace Beagle.Render
{
    [BeagleModule]
    class Module : Beagle.Core.DefaultModule
    {
        override public void Startup()
        {
            csglLoadGlfw();
            glfwInit();
        }

        override public string GetLogPath()
        {
            return "";
        }

        override public string GetLogPrefix()
        {
            return "Render";
        }
    }
}
