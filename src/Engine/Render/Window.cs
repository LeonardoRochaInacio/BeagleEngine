using System;
using Beagle.Core;
using static CSGL.OpenGL;
using static CSGL.CSGL;
using static CSGL.Glfw3;

namespace Beagle.Render
{
    public class Window
    {
        private IntPtr WindowInstance; 

        public Window(int Width, int Height, string Title)
        {
            glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 4);
            glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 4);

            WindowInstance = glfwCreateWindow(Width, Height, Title, IntPtr.Zero, IntPtr.Zero);
            
            if(WindowInstance == null)
            {
                Log.Error("Error on window creation");
                return;
            }
            else
            {
                Log.Success("Window {0} created, width: {1}, height: {2}", Title, Width, Height);
            }

            glfwMakeContextCurrent(WindowInstance);

            csglLoadGL();

            glClearColor(255.0f, 0.0f,0.0f , 1.0f);

            while (glfwWindowShouldClose(WindowInstance) == 0)
            {
                glfwPollEvents();

                glClear(GL_COLOR_BUFFER_BIT);

                glfwSwapBuffers(WindowInstance);
            }

            glfwTerminate();

        }

    }
}
