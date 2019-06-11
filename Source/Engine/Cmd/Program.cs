using System;
using Beagle.Core;
using Beagle.Application;

namespace Beagle.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ModuleManager.StartupModules();
                Window x = new Window(800,600,"");
                while (x.WindowUpdate()) ;
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
