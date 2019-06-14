using System;
using System.Text;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Beagle.Core
{
    /// <summary>
    /// Static beagle log class
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// Extesion of log files.
        /// </summary>
        static private string LogsExtension = "txt";

        /// <summary>
        /// General string builder, when module type isnt specificated, its will be inserted on this string builder.
        /// </summary>
        private static readonly StringBuilder GeneralLoggerString = new StringBuilder();

        /// <summary>
        /// Write a log without line break.
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Color"></param>
        /// <param name="CallerAssembly"></param>
        /// <param name="ShowTime"></param>
        /// <param name="Params"></param>
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

        /// <summary>
        /// Write a log with line break included.
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Color"></param>
        /// <param name="CallerAssembly"></param>
        /// <param name="ShowTime"></param>
        /// <param name="Params"></param>
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

        /// <summary>
        /// Assign a warning log.
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Params"></param>
        public static void Warning(string Text, params object[] Params)
        { 
            WriteLine(Text, ConsoleColor.DarkYellow, Assembly.GetCallingAssembly(), true, Params);
        }

        /// <summary>
        /// Assign a error log.
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Params"></param>
        public static void Error(string Text, params object[] Params)
        {
            WriteLine(Text, ConsoleColor.DarkRed, Assembly.GetCallingAssembly(), true, Params);
        }

        /// <summary>
        /// Assign a info log.
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Params"></param>
        public static void Info(string Text, params object[] Params)
        {
            WriteLine(Text, ConsoleColor.White, Assembly.GetCallingAssembly(), true, Params);
        }

        /// <summary>
        /// Assign a sucess log
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Params"></param>
        public static void Success(string Text, params object[] Params)
        {
            WriteLine(Text, ConsoleColor.Green, Assembly.GetCallingAssembly(), true, Params);
        }

        /// <summary>
        /// Assign a exception log
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Params"></param>
        public static void Exception(string Text, params object[] Params)
        {
            WriteLine(Text, ConsoleColor.DarkGray, Assembly.GetCallingAssembly(), true, Params);
        }

        /// <summary>
        /// Save a specific module on its specific anme and location previously preseted
        /// </summary>
        /// <param name="Module"></param>
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

                    try
                    {
                        Directory.CreateDirectory(Path.GetEngineLog());
                        File.WriteAllText(CompletePath, DefaultModuleInstance.LoggerString.ToString());
                    }
                    catch(Exception Ex)
                    {
                        Log.Exception(Ex.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Save general log (GeneralLoggerString)
        /// </summary>
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

                Directory.CreateDirectory(Path.GetEngineLog());
                File.WriteAllText(CompletePath, GeneralLoggerString.ToString());
            }
        }

        /// <summary>
        /// Save all logs. (All modules and general).
        /// </summary>
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
