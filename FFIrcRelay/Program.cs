using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using FFIrcRelay.Framework;

namespace FFIrcRelay
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (args.Contains("-install", StringComparer.InvariantCultureIgnoreCase))
            {
                CustomServiceInstaller.RuntimeInstall<ServiceImplementation>();
            }
            else if (args.Contains("-uninstall", StringComparer.InvariantCultureIgnoreCase))
            {
                CustomServiceInstaller.RuntimeUnInstall<ServiceImplementation>();
            }
            else
            {

                using (var implementation = new ServiceImplementation())
                {
                    if (Environment.UserInteractive)
                    {
                        ConsoleHarness.Run(args, implementation);
                    }
                    else
                    {
                        ServiceBase.Run(new ServiceHarness(implementation));
                    }
                }
            }
        }
    }
}
