using System;
using Beagle.Core;
using Beagle.GUI;
using GLFW;
using static OpenGL.Gl;

namespace Beagle.Application
{
    public class Window
    {
        public GLFW.Window WindowInstance;
        public readonly int width;
        public readonly int height;
        public readonly string title;
        public delegate void Del(string message);

        private void WindowHint()
        {
            Glfw.WindowHint(Hint.ClientApi, ClientApi.OpenGL);
            Glfw.WindowHint(Hint.ContextVersionMajor, 3);
            Glfw.WindowHint(Hint.ContextVersionMinor, 3);
            Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);
            Glfw.WindowHint(Hint.Doublebuffer, true);
        }

        private bool WindowCreationCheck()
        {
            if (WindowInstance == null)
            {
                Beagle.Core.Log.Error("Error on window creation");
                return false;
            }
            else
            {
                Beagle.Core.Log.Success("Window {0} created, width: {1}, height: {2}", title, width, height);
                return true;
            }
        }

        public Window(int Width, int Height, string Title)
        {
            title = Title;
            width = Width;
            height = Height;
            WindowHint();
            WindowInstance = Glfw.CreateWindow(Width, Height, Title, Monitor.None, GLFW.Window.None);
            if (!WindowCreationCheck()) return;
        }

        public virtual void WindowFrameSize_Callback(int Width, int Height)
        {
            glViewport(0, 0, Width, Height);
        }

        public virtual void WindowInputProcess()
        {
            if (Glfw.GetKey(WindowInstance, Keys.Escape) == InputState.Press)
            {
                Glfw.SetWindowShouldClose(WindowInstance, true);
            }
        }

        public bool WindowUpdate()
        {
            if (Glfw.WindowShouldClose(WindowInstance) == false)
            {

                Glfw.MakeContextCurrent(WindowInstance);

                glClearColor(1.0f, 0, 0, 1);
                glClear(GL_COLOR_BUFFER_BIT);

                WindowInputProcess();

                Glfw.SwapBuffers(WindowInstance);
                Glfw.PollEvents();

                return true;
            }
            else
            {
                Glfw.HideWindow(WindowInstance);
                return false;
            }
        }
    }
}
