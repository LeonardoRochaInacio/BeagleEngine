using System;
using Beagle.Core;
using Beagle.Application;
using Beagle.GUI;
using static CSGL.Glfw3;
using static CSGL.CSGL;
using static CSGL.OpenGL;
using GlmSharp;

namespace Beagle.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                
                Log.Info("Trying load and init Glfw");
                csglLoadGlfw();
                if (glfwInit() <= 0)
                {
                    Log.Error("Error on glfwInit()");
                    return;
                }

                    

                Window x = new Window(800,600,"");
                Console.WriteLine(x.width);

                glfwMakeContextCurrent(x.WindowInstance);

                csglLoadGL();

                if (x.WindowInstance == null)
                {
                    Log.Exception("PPAU");
                }

                float[] vertices;
                vertices = new float[6] {
                     0.0f, 0.5f, // Vertex 1 (X, Y)
                     0.5f, -0.5f, // Vertex 2 (X, Y)
                    -0.5f, -0.5f  // Vertex 3 (X, Y)
                };

                uint VS = csglShaderFile("C:/Users/Leonardo/Documents/BeagleEngine/gui.vs", GL_VERTEX_SHADER);
                uint FS = csglShaderFile("C:/Users/Leonardo/Documents/BeagleEngine/gui.fs", GL_FRAGMENT_SHADER);
                uint shaderprogram = csglShaderProgram(VS, FS);

                glBindFragDataLocation(shaderprogram, 0, "outColor");

                glUseProgram(shaderprogram);

                uint posAttrib = (uint)glGetAttribLocation(shaderprogram, "position");
                glEnableVertexAttribArray(posAttrib);

                uint bf = csglBuffer(vertices, 0);
                mat4 proj = mat4.Ortho(0.0f, 800.0f, 600.0f, 0.0f, -1.0f, 1.0f);


                csglVertexAttribPointer(posAttrib, 2, GL_FLOAT, false, 0, 0);

                glEnableVertexAttribArray(posAttrib);

                while (glfwWindowShouldClose(x.WindowInstance) <= 0)
                {

                    glClearColor(1.0f, 0.0f, 0.0f, 1.0f);
                    glClear(GL_COLOR_BUFFER_BIT);


                    

                    glDrawArrays(GL_TRIANGLES, 0, 3);

                    glfwSwapBuffers(x.WindowInstance);
                    glfwPollEvents();

                }

            }
            catch(Exception e)
            {
                Log.Exception(e.ToString());
                Log.SaveAllLogs(); new Exception();
            }

            while (true) ;

        }
    }
}
