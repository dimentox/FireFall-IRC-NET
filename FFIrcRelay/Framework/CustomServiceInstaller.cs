using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;

namespace FFIrcRelay.Framework
{
    [RunInstaller(true)]
    public partial class CustomServiceInstaller : Installer
    {
        public ServiceAttribute Configuration { get; set; }
        // Creates a blank Custom service installer with configuration in ServiceImplementation
        public CustomServiceInstaller()
            : this(typeof(ServiceImplementation))
        {
        }

        // Creates a Custom service installer using the type specified.
        public CustomServiceInstaller(Type CustomServiceType)
        {
            if (!CustomServiceType.GetInterfaces().Contains(typeof(IService)))
            {
                throw new ArgumentException("Type to install must implement IService.",
                                            "CustomServiceType");
            }

            var attribute = CustomServiceType.GetAttribute<ServiceAttribute>();

            if (attribute == null)
            {
                throw new ArgumentException("Type to install must be marked with a ServiceAttribute.",
                                            "CustomServiceType");
            }

            Configuration = attribute;
        }

        // Performs a transacted installation at run-time of the AutoCounterInstaller and any other listed installers.
        public static void RuntimeInstall<T>()
            where T : IService
        {
            string path = "/assemblypath=" + Assembly.GetEntryAssembly().Location;

            using (var ti = new TransactedInstaller())
            {
                ti.Installers.Add(new CustomServiceInstaller(typeof(T)));
                ti.Context = new InstallContext(null, new[] { path });
                ti.Install(new Hashtable());
            }
        }

        // Performs a transacted un-installation at run-time of the AutoCounterInstaller and any other listed installers.
        public static void RuntimeUnInstall<T>(params Installer[] otherInstallers)
            where T : IService
        {
            string path = "/assemblypath=" + Assembly.GetEntryAssembly().Location;

            using (var ti = new TransactedInstaller())
            {
                ti.Installers.Add(new CustomServiceInstaller(typeof(T)));
                ti.Context = new InstallContext(null, new[] { path });
                ti.Uninstall(null);
            }
        }

        // Installer class, to use run InstallUtil against this .exe
        public override void Install(System.Collections.IDictionary savedState)
        {
            ConsoleHarness.WriteToConsole(ConsoleColor.White, "Installing service {0}.", Configuration.Name);

            // install the service 
            ConfigureInstallers();
            base.Install(savedState);
        }

        // Removes the counters, then calls the base uninstall.
        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            ConsoleHarness.WriteToConsole(ConsoleColor.White, "Un-Installing service {0}.", Configuration.Name);

            // load the assembly file name and the config
            ConfigureInstallers();
            base.Uninstall(savedState);
        }

        // Method to configure the installers
        private void ConfigureInstallers()
        {
            // load the assembly file name and the config
            Installers.Add(ConfigureProcessInstaller());
            Installers.Add(ConfigureServiceInstaller());
        }

        // Helper method to configure a process installer for this Custom service
        private ServiceProcessInstaller ConfigureProcessInstaller()
        {
            var result = new ServiceProcessInstaller();

            // if a user name is not provided, will run under local service acct
            if (string.IsNullOrEmpty(Configuration.UserName))
            {
                result.Account = ServiceAccount.LocalService;
                result.Username = null;
                result.Password = null;
            }
            else
            {
                // otherwise, runs under the specified user authority
                result.Account = ServiceAccount.User;
                result.Username = Configuration.UserName;
                result.Password = Configuration.Password;
            }

            return result;
        }

        // Helper method to configure a service installer for this Custom service
        private ServiceInstaller ConfigureServiceInstaller()
        {
            // create and config a service installer
            var result = new ServiceInstaller
            {
                ServiceName = Configuration.Name,
                DisplayName = Configuration.DisplayName,
                Description = Configuration.Description,
                StartType = Configuration.StartMode,
            };

            return result;
        }
    }
}
