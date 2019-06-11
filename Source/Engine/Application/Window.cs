using System;
using Beagle.Core;
using static CSGL.OpenGL;
using static CSGL.Glfw3;

namespace Beagle.Application
{
    public class Window
    {
        public IntPtr WindowInstance;
        public readonly int width;
        public readonly int height;
        public readonly string title;
        public delegate void Del(string message);

        private void WindowHint()
        {
            glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 4);
            glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 0);
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

        public virtual void WindowFrameSize_Callback(int Width, int Height)
        {
            glViewport(0, 0, Width, Height);
        }

        public virtual void WindowInputProcess()
        {
            if (glfwGetKey(WindowInstance, GLFW_KEY_ESCAPE) == GLFW_PRESS)
            {
                glfwSetWindowShouldClose(WindowInstance, 1);
            }
        }

        public bool WindowUpdate()
        {
            if (glfwWindowShouldClose(WindowInstance) <= 0)
            {
                glfwMakeContextCurrent(WindowInstance);
                WindowInputProcess();
                glfwSwapBuffers(WindowInstance);
                glfwPollEvents();
                return true;
            }
            else
            {
                glfwHideWindow(WindowInstance);
                return false;
            }
        }
    }
}
