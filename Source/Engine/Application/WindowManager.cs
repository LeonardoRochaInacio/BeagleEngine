using System.Collections.Generic;
using Beagle.Core;

namespace Beagle.Application
{
    /// <summary>
    /// Not working yet.
    /// </summary>
    public static class WindowManager
    {
        public readonly static List<Window> Windows = new List<Window>();
        public static Window CreateWindow(string Title, int Width, int Height)
        {
            if(Windows.Count == 0)
            {
                Window NewWindow = new Window(Width, Height, Title);
                Windows.Add(NewWindow);
                return NewWindow;
            }
            else
            {
                Log.Error("Beagle not yet supports multi window creation");
            }

            return null;
        }

        public static Window GetMainWindow()
        {
            return Windows[0];
        }
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
