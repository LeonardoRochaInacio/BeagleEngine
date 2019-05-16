using System;
using Beagle.Core;
using static CSGL.OpenGL;
using static CSGL.CSGL;
using static CSGL.Glfw3;

namespace Beagle.Render
{
    public sealed class Window
    {
        public readonly IntPtr WindowInstance;
        public readonly int width;
        public readonly int height;
        public readonly string title;

        private void WindowHint()
        {
            glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 5);
            glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 4);
        }

        private bool WindowCreationCheck()
        {
            if (WindowInstance == null)
            {
                Log.Error("Error on window creation");
                return false;
            }
            else
            {
                Log.Success("Window {0} created, width: {1}, height: {2}", title, width, height);
                return true;
            }
        }

        public Window(int Width, int Height, string Title, Window WindowShare)
        {
            title = Title;
            width = Width;
            height = Height;
            WindowHint();
            WindowInstance = glfwCreateWindow(Width, Height, Title, IntPtr.Zero, WindowShare.WindowInstance);
            if (!WindowCreationCheck()) return;
        }

        public Window(int Width, int Height, string Title)
        {
            title = Title;
            width = Width;
            height = Height;
            WindowHint();
            WindowInstance = glfwCreateWindow(Width, Height, Title, IntPtr.Zero, IntPtr.Zero);
            if (!WindowCreationCheck()) return;
        }

        /*
         * 
         * 
         * glfwMakeContextCurrent(WindowInstance);

            csglLoadGL();

            glClearColor(255.0f, 0.0f,0.0f , 1.0f);

            while (glfwWindowShouldClose(WindowInstance) == 0)
            {
                glfwPollEvents();

                glClear(GL_COLOR_BUFFER_BIT);

                glfwSwapBuffers(WindowInstance);
            }

            glfwTerminate();

    */

    }
}
