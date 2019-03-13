using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;

namespace Beagle.Core
{
    public struct BeagleModule
    {
        private Assembly Assembly;

        public readonly Type Type;

        private readonly object ModuleInstance;

        public bool GetModuleInstance(out object _ModuleInstance)
        {
            _ModuleInstance = null;
            if (ModuleInstance != null)
            {
                _ModuleInstance = ModuleInstance;
                return true;
            }

            return false;
        }

        public BeagleModule(Assembly assembly, Type type, object moduleInstance)
        {
            Assembly = assembly;
            Type = type;
            ModuleInstance = moduleInstance;
        }
    }

    public static class ModuleManager
    {
        public static readonly List<BeagleModule> Modules = new List<BeagleModule>();

        static ModuleManager()
        {
            foreach (Assembly ASS in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type Type in ASS.GetTypes())
                {
                    if(Type.GetCustomAttributes(typeof(BeagleModuleAttribute), true).Length > 0)
                    {
                        if(Type.GetInterfaces().Length > 0 && Type.GetInterfaces()[0].Equals(typeof(IDefaultModule)))
                        {
                            Log.Success("Module {0} recognized.", Type.Module.Name);
                            Modules.Add(new BeagleModule(ASS, Type, Activator.CreateInstance(Type)));
                        }
                        else
                        {
                            Log.Error("Module interface not implemented for {0}", Type.Module.Name);
                        }
                    }
                    
                }
            }
        }

        public static void StartupModules()
        {
            foreach (BeagleModule Module in Modules)
            {
                object AlreadyLoadedModule;
                if (Module.GetModuleInstance(out AlreadyLoadedModule))
                {
                    Module.Type.InvokeMember("Startup", BindingFlags.InvokeMethod, null, AlreadyLoadedModule, new object[] { });
                }
                else
                {
                    Log.Error("Module {0} impossible to startup", Module.Type.Module.Name);
                }
            }
        }
    }
}
