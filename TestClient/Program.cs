using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Microsoft.AspNet.SignalR.Client.Transports;
namespace TestClient
{
    class Program
    {
        Microsoft.AspNet.SignalR.Client.Connection conn = new Microsoft.AspNet.SignalR.Client.Connection("http://localhost:8181/echo");
        static void Main(string[] args)
        {

            Console.WriteLine("Press key to start...");
            Console.ReadKey();
            // Connect to the service
            var hubConnection = new HubConnection("http://localhost:8181/signalr");
            //var hubConnection = new HubConnection("http://ipv4.fiddler:8082/signalr");


            // Create a proxy to the chat service
            var chat = hubConnection.CreateHubProxy("ircHub");

            // Print the message when it comes in
            chat.On("onServerMsg", msg => Console.WriteLine(msg));
            chat.On<string, string>("onJoin", (nick, msg) => Console.WriteLine(nick + " " + msg));
            chat.On<string, string>("onPart", (nick, msg) => Console.WriteLine(nick + " " + msg));
            chat.On<string, string>("onMEAction", (nick, msg) => Console.WriteLine(nick + " --- " + msg));
            chat.On<string, string[]>("onNames", (chan, names) => {
                Console.WriteLine("In Channel");
                foreach (var item in names)
                {
                    Console.WriteLine(item);
                }
            });
            chat.On<string, string, string>("onMessage", (a,b,c) => Console.WriteLine("{0} - {1}: {2}", a, b, c));


            // Start the connection
            hubConnection.Start(new LongPollingTransport()).Wait();
            chat.Invoke("ircConnect", "irc.globalgamers.net", "#test123", "thisisatets", "");

            string line = null;
            while ((line = Console.ReadLine()) != null)
            {
                // Send a message to the server
                chat.Invoke("say", line).ContinueWith(t =>
                {
                    Console.WriteLine("Invoke finished");
                });
                //chat.Invoke<string>("Send2", line).ContinueWith(t =>
                //{
                //    Console.WriteLine("Return value: " + t.Result);
                //});
            }
            chat.Invoke("ircDisconnect");
        }
    }
}
