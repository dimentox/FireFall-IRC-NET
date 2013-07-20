using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFIrcRelay
{

    public class IrcHub: Hub
    {
        private readonly Manager _manager;
       
        public IrcHub() : this(Manager.Instance) { }

        public IrcHub(Manager mgr)
        {
            _manager = mgr;
        }

        public override Task OnConnected()
        {
            Console.WriteLine("Got COnnection " + Context.ConnectionId);
            Clients.Caller.onServerMsg("Welcome!");
            _manager.ClientConnect(Context.ConnectionId);
            return base.OnConnected();
        }
        public override Task OnDisconnected()
        {
            _manager.ClientDisconnect(Context.ConnectionId);
            return base.OnDisconnected();
        }
        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }

      

        public void ircConnect(string host, string chan, string nick, string pass) 
        {
            _manager.ircConnect(Context.ConnectionId, host, chan, nick, pass);
        }
        public void ircDisconnect()
        {
            _manager.ircDisconnect(Context.ConnectionId);
        }
        public void ircRaw()
        {
            _manager.ircRaw(Context.ConnectionId);
        }
        public void say(string msg)
        {
            Console.WriteLine("Got MSG" + msg);
            _manager.say(Context.ConnectionId, msg);
        }
       
    }
}
