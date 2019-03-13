using System;
using System.Text;
using System.IO;
using System.Reflection;

namespace Beagle.Core
{
    public static class Log
    {
        static private readonly StringBuilder Logger = new StringBuilder();

        public static void Write(string Text, ConsoleColor Color, bool ShowTime = true, params object[] Params)
        {
            string CompleteString = (ShowTime ? DateTime.Now.ToString("[HH:mm:ss]: ") : "") + Text;
            Console.ForegroundColor = Color;
            Console.Write(CompleteString, Params);
            Logger.Append(CompleteString);
            Console.ResetColor();
        }

        public static void WriteLine(string Text, ConsoleColor Color, bool ShowTime = true, params object[] Params)
        {
            string CompleteString = (ShowTime ? DateTime.Now.ToString("[HH:mm:ss]: ") : "") + Text;
            Console.ForegroundColor = Color;
            Console.WriteLine(CompleteString, Params);
            Logger.Append(CompleteString);
            Console.ResetColor();
        }

        public static void Warning(string Text, params object[] Params)
        {
            WriteLine(Text, ConsoleColor.DarkYellow, true, Params);
        }

        public static void Error(string Text, params object[] Params)
        {
            WriteLine(Text, ConsoleColor.DarkRed, true, Params);
        }

        public static void Info(string Text, params object[] Params)
        {
            WriteLine(Text, ConsoleColor.White, true, Params);
        }

        public static void Success(string Text, params object[] Params)
        {
            WriteLine(Text, ConsoleColor.Green, true, Params);
        }

        public static void Exception(string Text, params object[] Params)
        {
            WriteLine(Text, ConsoleColor.DarkGray, true, Params);
        }

        public static void SaveLog(string FilePath, string FileName)
        {
            string Extension = "txt";
            string CompletePath = FilePath + FileName + "." + Extension;
            if (File.Exists(CompletePath))
            {
                File.AppendAllText(Logger.ToString(), CompletePath);
            }
            else
            {
                File.WriteAllText(Logger.ToString(), CompletePath);
            }
        }
    }
}
