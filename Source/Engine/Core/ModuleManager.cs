using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;

namespace Beagle.Core
{
    /// <summary>
    /// Struct that represets a module.
    /// </summary>
    public struct RecognizedBeagleModule
    {
        /// <summary>
        /// Type of module 
        /// </summary>
        public readonly Type ModuleType;

        /// <summary>
        /// stores module instance
        /// </summary>
        private readonly DefaultModule ModuleInstance;

        /// <summary>
        /// Get module instance
        /// </summary>
        /// <param name="_ModuleInstance"></param>
        /// <returns></returns>
        public bool GetModuleInstance(out DefaultModule _ModuleInstance)
        {
            _ModuleInstance = null;
            if (ModuleInstance != null)
            {
                _ModuleInstance = ModuleInstance;
                return true;
            }

            return false;
        }

        /// <summary>
        /// BeagleModule constructor.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="type"></param>
        /// <param name="moduleInstance"></param>
        public RecognizedBeagleModule(Type type, DefaultModule moduleInstance)
        {
            ModuleType = type;
            ModuleInstance = moduleInstance;
        }
    }

    /// <summary>
    /// ModuleManager class
    /// </summary>
    public static class ModuleManager
    {
        /// <summary>
        /// Save all recognized modules.
        /// </summary>
        public static readonly Dictionary<Assembly, RecognizedBeagleModule> Modules = new Dictionary<Assembly, RecognizedBeagleModule>();

        /// <summary>
        /// ModuleManager constructor
        /// </summary>
        static ModuleManager()
        {
            ModuleRecognization();
        }

        /// <summary>
        /// Recognization loop based on tag and implementation on idefaultmodule.
        /// </summary>
        private static void ModuleRecognization()
        {
            foreach (Assembly ASS in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type Type in ASS.GetTypes())
                {
                    if (Type.GetCustomAttributes(typeof(BeagleModule), true).Length > 0)
                    {
                            if (Assembly.GetEntryAssembly() != ASS)
                                Log.Success("Module {0} recognized.", Type.Module.Name.Replace(".dll", ""));
                            else
                                Log.Success("Application initialized based on {0} module.", Type.Module.Name.Replace(".exe", "").Replace(".dll", ""));
                            try
                            {
                                Modules.Add(ASS, new RecognizedBeagleModule(Type, (DefaultModule)Activator.CreateInstance(Type)));
                            }
                            catch(ArgumentException)
                            {
                                string Ex = "Possible duplicate [BeagleModule] attribute on class, this may be caused because DefaultModule.cs has declared a [BeagleModule] attribute.";
                                Log.Exception(Ex);
                                throw new ArgumentException(Ex);
                            }
                            catch(InvalidCastException)
                            {
                                string Ex = "Cast to DefaultModule error. Check if the Module class of {0} has a DefaultClass as base class. Use inheritance of Default class instead implements raw IDefaultModule interface.";
                                Log.Exception(Ex);
                                throw new InvalidCastException(Ex);
                            }
                    }
                }
            }
        }

        /// <summary>
        /// Invoke all startup modules method.
        /// </summary>
        public static void StartupModules()
        {
            foreach (RecognizedBeagleModule Module in Modules.Values)
            {
                DefaultModule AlreadyLoadedModule;
                if (Module.GetModuleInstance(out AlreadyLoadedModule))
                {
                    Log.Info("Starting up module {0}...", Module.ModuleType.Module.Name.Replace(".exe", "").Replace(".dll", ""));
                    try
                    {
                        Module.ModuleType.InvokeMember("Startup", BindingFlags.InvokeMethod, null, AlreadyLoadedModule, new object[] { });
                        Log.Success("Module {0} started!", AlreadyLoadedModule.GetModuleName());
                    }
                    catch(System.NullReferenceException Ex)
                    {
                        Log.Error(Ex.ToString());
                    }
                }
                else
                {
                    Log.Error("Module {0} impossible to startup", Module.ModuleType.Module.Name.Replace(".exe", "").Replace(".dll", ""));
                }
            }
        }
    }
}
