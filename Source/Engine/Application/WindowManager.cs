using System.Collections.Generic;
using Beagle.Core;
using static CSGL.Glfw3;
using static CSGL.OpenGL;

namespace Beagle.Application
{
    /// <summary>
    /// Not working yet.
    /// </summary>
    public static class WindowManager
    {
        public readonly static List<Window> Windows = new List<Window>();
        static WindowManager()
        {
            CreateWindow("Beagle Default Window", 1024, 1024);
        }

        public static void CreateWindow(string Title, int Width, int Height)
        {
            if(Windows.Count == 0)
            {
                Windows.Add(new Window(Width, Height, Title));
            }
            else
            {
                Log.Error("Beagle not yet supports multi window creation");
            }
        }

        public static Window GetMainWindow()
        {
            return Windows[0];
        }

        public static void AddWindowToManager(Window Window) { }

        public static void GlobalWindowUpdate()
        {
            /*while (true)
            {
                foreach (Window Window in Windows)
                {
                    Window.WindowUpdate();
                    glfwPollEvents();
                }
            }*/
        }
    }
}
