using System;
using System.Text;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Beagle.Core
{
    public static class Log
    {

        static private string LogsExtension = "txt";

        private static readonly StringBuilder GeneralLoggerString = new StringBuilder();

        public static void Write(string Text, ConsoleColor Color, Assembly CallerAssembly, bool ShowTime = true, params object[] Params)
        {
            DefaultModule Defaults = null;
            string CallerName;
            bool SearchedModule = ModuleManager.Modules.ContainsKey(CallerAssembly);

            if (SearchedModule)
            {
                ModuleManager.Modules[CallerAssembly].GetModuleInstance(out Defaults);
                CallerName = Defaults.GetLogPrefix();
            }
            else
            {
                CallerName = "General";
            }

            string CompleteString = (ShowTime ? DateTime.Now.ToString("[HH:mm:ss]: ") : "") + Text;
            Console.ForegroundColor = Color;
            Console.Write("[" + CallerName + "]" + CompleteString, Params);

            if (SearchedModule)
            {
                Defaults.LoggerString.AppendLine(string.Format(CompleteString, Params));
            }
            else
            {
                GeneralLoggerString.AppendLine(string.Format(CompleteString, Params));
            }

            Console.ResetColor();
        }

        public static void WriteLine(string Text, ConsoleColor Color, Assembly CallerAssembly, bool ShowTime = true, params object[] Params)
        {
            DefaultModule Defaults = null;
            string CallerName;
            bool SearchedModule = ModuleManager.Modules.ContainsKey(CallerAssembly);

            if (SearchedModule)
            {
                ModuleManager.Modules[CallerAssembly].GetModuleInstance(out Defaults);
                CallerName = Defaults.GetLogPrefix();
            }
            else
            {
                CallerName = "General";
            }

            string CompleteString = (ShowTime ? DateTime.Now.ToString("[HH:mm:ss]: ") : "") + Text;
            Console.ForegroundColor = Color;
            Console.WriteLine("[" + CallerName + "]" + CompleteString, Params);
   
            if (SearchedModule)
            {
                Defaults.LoggerString.AppendLine(string.Format(CompleteString, Params));
            }
            else
            {
                GeneralLoggerString.AppendLine(string.Format(CompleteString, Params));
            }

            Console.ResetColor();
        }

        public static void Warning(string Text, params object[] Params)
        { 
            WriteLine(Text, ConsoleColor.DarkYellow, Assembly.GetCallingAssembly(), true, Params);
        }

        public static void Error(string Text, params object[] Params)
        {
            WriteLine(Text, ConsoleColor.DarkRed, Assembly.GetCallingAssembly(), true, Params);
        }

        public static void Info(string Text, params object[] Params)
        {
            WriteLine(Text, ConsoleColor.White, Assembly.GetCallingAssembly(), true, Params);
        }

        public static void Success(string Text, params object[] Params)
        {
            WriteLine(Text, ConsoleColor.Green, Assembly.GetCallingAssembly(), true, Params);
        }

        public static void Exception(string Text, params object[] Params)
        {
            WriteLine(Text, ConsoleColor.DarkGray, Assembly.GetCallingAssembly(), true, Params);
        }

        public static void SaveModuleLog(BeagleModule Module)
        {
            if (Module.Equals(null)) return;
            DefaultModule DefaultModuleInstance;
            bool SearchedModule = Module.GetModuleInstance(out DefaultModuleInstance);

            if (SearchedModule)
            {
                string CompletePath = "";
                string SemiPath = DefaultModuleInstance.GetModuleName() + "." + LogsExtension;
                if (DefaultModuleInstance.GetLogPath() == "" || DefaultModuleInstance.GetLogPath() == null)
                {
                    CompletePath = Path.GetEngineLog() + SemiPath;
                }
                else
                {
                    CompletePath = DefaultModuleInstance.GetLogPath() + SemiPath;
                } 

                if (File.Exists(CompletePath))
                {
                    File.AppendAllText(CompletePath, DefaultModuleInstance.LoggerString.ToString());
                }
                else
                {
                    
                    System.IO.Directory.CreateDirectory(Path.GetEngineLog());
                    File.WriteAllText(CompletePath, DefaultModuleInstance.LoggerString.ToString());
                }
            }
        }

        public static void SaveGeneralLog()
        {
            string SemiPath = "General" + "." + LogsExtension;
            string CompletePath = Path.GetEngineLog() + SemiPath;
            if (File.Exists(CompletePath))
            {
                File.AppendAllText(CompletePath, GeneralLoggerString.ToString());
            }
            else
            {

                System.IO.Directory.CreateDirectory(Path.GetEngineLog());
                File.WriteAllText(CompletePath, GeneralLoggerString.ToString());
            }
        }

        public static void SaveAllLogs()
        {
            foreach (BeagleModule Module in ModuleManager.Modules.Values)
            {
                    Log.SaveModuleLog(Module);
            }
            SaveGeneralLog();
        }    
    }
}
