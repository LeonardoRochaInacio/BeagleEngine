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
        static Path(){ }

        static public char GetPathDirectorySeparator()
        {
            return '/';
        }

        static public String FixPathSeparators(String _Path)
        {
            return _Path.Replace('\\', GetPathDirectorySeparator()) + "/";
        }

        /// <summary>
        /// Get current directory path as String
        /// </summary>
        /// <returns></returns>
        static public String CurrentDirectory()
        {
            return FixPathSeparators(Environment.CurrentDirectory);
        }

        /// <summary>
        /// Get Documents directory path as String
        /// </summary>
        /// <returns></returns>
        static public String GetMyDocuments()
        {
            return FixPathSeparators(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
        }

        /// <summary>
        /// Get Common Documents directory path as String
        /// </summary>
        /// <returns></returns>
        static public String GetCommonDocuments()
        {
            return FixPathSeparators(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments));
        }

        /// <summary>
        /// Get Program Files directory path as String
        /// </summary>
        /// <returns></returns>
        static public String GetProgramFiles()
        {
            return FixPathSeparators(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
        }

        /// <summary>
        /// Get Program Files x86 directory path as String
        /// </summary>
        /// <returns></returns>
        static public String GetProgramFilesx86()
        {
            return FixPathSeparators(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));
        }

        /// <summary>
        /// Get Engine Root directory path as String
        /// </summary>
        /// <returns></returns>
        static public String GetEngineRoot()
        {
            return FixPathSeparators(System.IO.Path.GetFullPath("../../../../.."));
        }

        /// <summary>
        /// Get Engine Binaries directory path as String
        /// </summary>
        /// <returns></returns>
        static public String GetEngineBin()
        {
            return FixPathSeparators(GetEngineRoot() + "bin");
        }

        /// <summary>
        /// Get Engine Resources directory path as String
        /// </summary>
        /// <returns></returns>
        static public String GetEngineResource()
        {
            return FixPathSeparators(GetEngineRoot() + "Resource");
        }

        /// <summary>
        /// Get Engine Log directory path as String
        /// </summary>
        /// <returns></returns>
        static public String GetEngineLog()
        {
            return FixPathSeparators(GetEngineRoot() + "Log");
        }

        /// <summary>
        /// Combine two paths
        /// </summary>
        /// <param name="Paths"></param>
        /// <returns></returns>
        static public String CombinePath(params string[] Paths)
        {
            return System.IO.Path.Combine(Paths);
        }
    }
}
