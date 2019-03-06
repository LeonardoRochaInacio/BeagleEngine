using System;

namespace Beagle.Core
{
    /// <summary>
    /// Static class to manage both system and engine paths
    /// </summary>
    public static class Path
    {

        /// <summary>
        /// Static path constructor
        /// </summary>
        static Path()
        {
            Console.WriteLine("Path constructor");
        }

        /// <summary>
        /// Get current directory path as String
        /// </summary>
        /// <returns></returns>
        static public String CurrentDirectory()
        {
            return Environment.CurrentDirectory;
        }

        /// <summary>
        /// Get Documents directory path as String
        /// </summary>
        /// <returns></returns>
        static public String GetDocuments()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        /// <summary>
        /// Get Common Documents directory path as String
        /// </summary>
        /// <returns></returns>
        static public String GetCommonDocuments()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
        }

        /// <summary>
        /// Get Program Files directory path as String
        /// </summary>
        /// <returns></returns>
        static public String GetProgramFiles()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        }

        /// <summary>
        /// Get Program Files x86 directory path as String
        /// </summary>
        /// <returns></returns>
        static public String GetProgramFilesx86()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        }

        /// <summary>
        /// Get Engine Root directory path as String
        /// </summary>
        /// <returns></returns>
        static public String GetEngineRoot()
        {
            return "";
        }

        /// <summary>
        /// Get Engine Binaries directory path as String
        /// </summary>
        /// <returns></returns>
        static public String GetEngineBin()
        {
            return "";
        }

        /// <summary>
        /// Get Engine Resources directory path as String
        /// </summary>
        /// <returns></returns>
        static public String GetEngineResource()
        {
            return "";
        }
    }
}
