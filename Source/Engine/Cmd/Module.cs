﻿using System;
using Beagle.Core;

namespace Beagle.Cmd
{
    [BeagleModule("Cmd")]
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
            return "Cmd";
        }
    }
}
