﻿using System;
using System.Collections.Generic;
using IniParser;
using IniParser.Model;
using IniParser.Exceptions;

namespace Beagle.Core
{
    /// <summary>
    /// File path handler to better manipulation
    /// </summary>
    public struct INIFile
    {
        public INIFile(string fileName) : this()
        {
            FilePath = fileName;
        }

        public readonly String FilePath;
    }

    /// <summary>
    /// Singleton ini parser class
    /// </summary>
    public sealed class IniParser
    {
        /// <summary>
        /// Singleton pattern
        /// </summary>
        private IniParser() { }

        private static IniParser Instance;

        public static IniParser SingletonInstance
        {
            get { return Instance ?? (Instance = new IniParser()); }
        }

        /// <summary>
        /// List of files previously loaded.
        /// </summary>
        Dictionary<String, IniData> LoadedIniFiles = new Dictionary<String, IniData>();

        /// <summary>
        /// Class specifiers
        /// </summary>
        private FileIniDataParser DataParser = new FileIniDataParser();

        /// <summary>
        /// Creats a ini file
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public INIFile CreateIniFile(String FilePath, String Name)
        {
            String CompleteFilePath = FilePath + Name + ".ini";    
            try
            {
                IniData Data = DataParser.ReadFile(CompleteFilePath);
                Console.WriteLine(String.Format("File {0} already exist. Returning self data.", CompleteFilePath));
                return new INIFile(CompleteFilePath);
            }
            catch(ParsingException)
            {
                IniData EmptyData = new IniData();
                DataParser.WriteFile(CompleteFilePath, EmptyData);
                return new INIFile(CompleteFilePath);
            }
        }

        /// <summary>
        /// Get key data
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="Section"></param>
        /// <param name="Key"></param>
        /// <param name="KeyData"></param>
        /// <returns></returns>
        public bool GetKeyData(String FilePath, String Section, String Key, out String KeyData)
        {
            IniData Data;
            if (CheckFileLoad(FilePath, out Data))
            {
                KeyData = Data[Section][Key];
                return true;
            }

            KeyData = "";
            return false;
        }

        /// <summary>
        /// Creates a section
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="SectionName"></param>
        public void CreateSection(String FilePath, String SectionName)
        {
            IniData Data;
            if (CheckFileLoad(FilePath, out Data)) Data.Sections.AddSection(SectionName);
        }

        /// <summary>
        /// Creates a section
        /// </summary>
        /// <param name="File"></param>
        /// <param name="SectionName"></param>
        public void CreateSection(INIFile File, String SectionName)
        {
            IniData Data;
            if (CheckFileLoad(File.FilePath, out Data)) Data.Sections.AddSection(SectionName);
        }

        /// <summary>
        /// Creaes a key
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="Section"></param>
        /// <param name="KeyName"></param>
        /// <param name="KeyData"></param>
        public void CreateKey(String FilePath, String Section, String KeyName, String KeyData)
        {
            IniData Data;
            if (CheckFileLoad(FilePath, out Data)) Data[Section].AddKey(KeyName, KeyData);
        }

        /// <summary>
        /// Creates a key
        /// </summary>
        /// <param name="File"></param>
        /// <param name="Section"></param>
        /// <param name="KeyName"></param>
        /// <param name="KeyData"></param>
        public void CreateKey(INIFile File, String Section, String KeyName, String KeyData)
        {
            IniData Data;
            if (CheckFileLoad(File.FilePath, out Data)) Data[Section].AddKey(KeyName, KeyData);
        }

        /// <summary>
        /// Remove a section
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="SectionName"></param>
        public void RemoveSection(String FilePath, String SectionName)
        {
            IniData Data;
            if (CheckFileLoad(FilePath, out Data)) Data.Sections.RemoveSection(SectionName);
        }

        /// <summary>
        /// Remove a section
        /// </summary>
        /// <param name="File"></param>
        /// <param name="SectionName"></param>
        public void RemoveSection(INIFile File, String SectionName)
        {
            IniData Data;
            if (CheckFileLoad(File.FilePath, out Data)) Data.Sections.RemoveSection(SectionName);
        }

        /// <summary>
        /// Remove a key
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="Section"></param>
        /// <param name="KeyName"></param>
        public void RemoveKey(String FilePath, String Section, String KeyName)
        {
            IniData Data;
            if (CheckFileLoad(FilePath, out Data)) Data[Section].RemoveKey(KeyName);
        }

        /// <summary>
        /// Remove a key
        /// </summary>
        /// <param name="File"></param>
        /// <param name="Section"></param>
        /// <param name="KeyName"></param>
        public void RemoveKey(INIFile File, String Section, String KeyName)
        {
            IniData Data;
            if (CheckFileLoad(File.FilePath, out Data)) Data[Section].RemoveKey(KeyName);
        }

        /// <summary>
        /// Save a ini file
        /// </summary>
        /// <param name="FilePath"></param>
        public void SaveFile(String FilePath)
        {
            IniData Data;
            if(CheckFileLoad(FilePath, out Data)) DataParser.WriteFile(FilePath, Data);                
        }

        /// <summary>
        /// Save a ini file
        /// </summary>
        /// <param name="File"></param>
        public void SaveFile(INIFile File)
        {
            IniData Data;
            if (CheckFileLoad(File.FilePath, out Data)) DataParser.WriteFile(File.FilePath, Data);
        }

        /// <summary>
        /// Check if ini file is loaded.
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="IniData"></param>
        /// <returns></returns>
        private bool CheckFileLoad(String FilePath, out IniData IniData)
        {
            if (LoadedIniFiles.ContainsKey(FilePath))
            {
                IniData = LoadedIniFiles[FilePath];
                return true;
            }
            else
            {
                try
                { 
                    IniData = DataParser.ReadFile(FilePath);
                    LoadedIniFiles.Add(FilePath, IniData);
                    return true;
                }
                catch(ParsingException)
                {
                    Console.WriteLine(String.Format("Failed to parse INI file: {0}", FilePath));
                    IniData = new IniData();
                    return false;
                }
            }
        }
    }
}
