using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace FFIrcRelay
{

    // implements IService (including AbstractCustomService) 
    public sealed partial class ServiceHarness : ServiceBase
    {
        // Get the class implementing the Custom service
        public IService ServiceImplementation { get; private set; }

        // Constructor a generic Custom service from the given class
        public ServiceHarness(IService serviceImplementation)
        {
            InitializeComponent();
            // make sure service passed in is valid
            if (serviceImplementation == null)
            {
                throw new ArgumentNullException("serviceImplementation",
                    "IService cannot be null in call to GenericService");
            }

            // set instance and backward instance
            ServiceImplementation = serviceImplementation;

            // configure our service
            ConfigureServiceFromAttributes(serviceImplementation);
        }




        // Override service control on continue
        protected override void OnContinue()
        {
            // perform class specific behavior 
            ServiceImplementation.OnContinue();
        }

        // Called when service is paused
        protected override void OnPause()
        {
            // perform class specific behavior 
            ServiceImplementation.OnPause();
        }


        // Called when a custom command is requested
        protected override void OnCustomCommand(int command)
        {
            // perform class specific behavior 
            ServiceImplementation.OnCustomCommand(command);
        }

        // Called when the Operating System is shutting down
        protected override void OnShutdown()
        {
            // perform class specific behavior
            ServiceImplementation.OnShutdown();
        }

        // Called when service is requested to start
        protected override void OnStart(string[] args)
        {
            ServiceImplementation.OnStart(args);
        }

        // Called when service is requested to stop
        protected override void OnStop()
        {
            ServiceImplementation.OnStop();
        }

        // Set configuration data
        private void ConfigureServiceFromAttributes(IService serviceImplementation)
        {
            var attribute = serviceImplementation.GetType().GetAttribute<ServiceAttribute>();

            if (attribute != null)
            {
                EventLog.Source = string.IsNullOrEmpty(attribute.EventLogSource)
                                    ? "ServiceHarness"
                                    : attribute.EventLogSource;

                CanStop = attribute.CanStop;
                CanPauseAndContinue = attribute.CanPauseAndContinue;
                CanShutdown = attribute.CanShutdown;

                // we don't handle: laptop power change event
                CanHandlePowerEvent = false;

                // we don't handle: Term Services session event
                CanHandleSessionChangeEvent = false;

                // always auto-event-log 
                AutoLog = true;
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format("IService implementer {0} must have a ServiceAttribute.",
                                  serviceImplementation.GetType().FullName));
            }
        }


    }
}
