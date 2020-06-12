using System;

namespace Beagle.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BeagleModule : Attribute
    {
        public String ModuleName;
        public String LogPrefix;
        public BeagleModule(String ModuleName)
        {
            this.ModuleName = ModuleName;
            this.LogPrefix = ModuleName;
        }

        public BeagleModule(String ModuleName, String LogPrefix)
        {
            this.ModuleName = ModuleName;
            this.LogPrefix = LogPrefix;
        }
    }
}