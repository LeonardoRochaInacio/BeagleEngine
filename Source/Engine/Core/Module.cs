namespace Beagle.Core
{
    [BeagleModule]
    class Module : DefaultModule
    {
        override public void Startup() 
        {
            
        }

        override public string GetLogPath()
        {
            return "";
        }

        override public string GetLogPrefix()
        {
            return "Core";
        }
    }
}
