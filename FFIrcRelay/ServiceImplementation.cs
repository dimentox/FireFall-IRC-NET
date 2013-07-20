using SelfHostSignalR20;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FFIrcRelay
{
    [Service("FFIrcRelay",
    DisplayName = "FFIrcRelay",
    Description = "The description of the FFIrcRelay service.",
    EventLogSource = "FFIrcRelay",
    StartMode = ServiceStartMode.Automatic)]
    public class ServiceImplementation : IService
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        Task ServerTask;
        public void OnStart(string[] args)
        {
            cts.Token.ThrowIfCancellationRequested();
            ServerTask = Task.Factory.StartNew(() =>
                {
                    Server srv = new Server();
                    srv.Run();
                }, cts.Token);
            Console.WriteLine("Server Started");
        }

        public void OnStop()
        {
            try
            {
                cts.Cancel();
            }
            catch
            {
                //This is ok to catch cause we want it to throw!
            }
            Console.WriteLine("Server Stopped");
        }

        public void OnPause()
        {

        }

        public void OnContinue()
        {

        }

        public void OnShutdown()
        {

        }

        public void Dispose()
        {

        }


        public void OnCustomCommand(int command)
        {

        }
    }
}
