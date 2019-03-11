using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beagle.Core;

namespace Beagle.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine(Path.CurrentDirectory());
            var x = IniParser.SingletonInstance;

            INIFile File =  x.CreateIniFile(Path.GetMyDocuments(), "leo");
            x.RemoveSection(File, "leonaro");
            x.SaveFile(File);
            while (true) ;
            
        }
    }
}
