using System;
using Beagle.Core;
using Beagle.Render;
using System.Reflection;

namespace Beagle.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {

            ModuleManager.StartupModules();

            Test.xxxx();

            Log.Info("Pla");
            Log.Info("Test");

            Log.SaveAllLogs();

            Console.ReadLine();
            
        }
    }
}
