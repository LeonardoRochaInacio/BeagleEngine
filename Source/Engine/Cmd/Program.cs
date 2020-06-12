using Beagle.Core;
using GLFW;

namespace Beagle.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                BeagleInitialization();
                GlfwMainInitialization();
                Beagle.Application.Window x = new Beagle.Application.Window(800,600,"");
            }
            catch(System.Exception e)
            {
                Log.Exception(e.ToString());
                Log.SaveAllLogs(); new System.Exception();
            }

            while (true) ;

        }

        static void GlfwMainInitialization()
        {
            Log.Info("Trying load and init Glfw");
            if (!Glfw.Init())
            {
                Log.Error("Error on glfwInit()");
                return;
            }
        }

        static void BeagleInitialization()
        {
            ModuleManager.StartupModules();
        }
    }
}
