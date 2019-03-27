using System;
using Beagle.Core;
using Beagle.Render;
using static CSGL.Glfw3;

namespace Beagle.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ModuleManager.StartupModules();
                Window x = new Window(800, 800, "Teste");
            }
            catch { Log.SaveAllLogs(); new Exception(); }

        }
    }
}
