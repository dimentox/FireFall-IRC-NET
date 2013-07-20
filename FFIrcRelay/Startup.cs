using Microsoft.AspNet.SignalR;
using Owin;
using System.IO;
using System.Reflection;
using Microsoft.Owin.Hosting;
using System;
namespace SelfHostSignalR20
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Turn cross domain on 
            var config = new HubConfiguration { EnableCrossDomain = true };
            // This will map out to http://localhost:8181/signalr 
            app.MapHubs(config);
            string exeFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string webFolder = Path.Combine(exeFolder, "Web");
            app.UseStaticFiles(webFolder);
        }
    }
    public class Server
    {
        string url = "http://localhost:8181";
        public void Run()
        {
            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine("Server running on {0}", url);
                Console.ReadLine();
            }
        }
    }
}