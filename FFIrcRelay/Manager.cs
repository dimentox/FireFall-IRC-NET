using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetIrc2;
using System.Collections.Concurrent;

namespace FFIrcRelay
{
    public class Manager
    {

        List<IrcWrapper> connections;
        public object Lck { get; set; }
        #region Init / Ctor etc
        private readonly static Lazy<Manager> _instance = new Lazy<Manager>(() => new Manager(GlobalHost.ConnectionManager.GetHubContext<IrcHub>().Clients));

        public static Manager Instance
        {
            get
            {
                return _instance.Value;
            }
        }
        private Manager(IHubConnectionContext clients)
        {
            Clients = clients;
            connections = new List<IrcWrapper>();
            Lck = new object();
            
        }
        private IHubConnectionContext Clients
        {
            get;
            set;
        }
        #endregion


        #region Connections
        public void ClientConnect(string connectionID)
        {
            lock (Lck)
            {
            var testConnection = this.connections.Where(o => o.connectionID == connectionID).FirstOrDefault();
            if (testConnection == null)
            {
                var client = this.Clients.Client(connectionID);
                IrcWrapper iwrap = new IrcWrapper(connectionID, client);
                connections.Add(iwrap);
            }
            }

           
        }
        public void ClientDisconnect(string connectionID)
        {
            lock (Lck)
            {
                var testConnection = this.connections.Where(o => o.connectionID == connectionID).FirstOrDefault();
                if (testConnection != null)
                {
                    testConnection.Disconnect();
                    connections.Remove(testConnection);
                }
            }

        }
        #endregion

        #region IrcCommands
        public void ircConnect(string connectionID, string host, string chan, string nick, string pass)
        {
            lock (Lck)
            {
                var testConnection = this.connections.Where(o => o.connectionID == connectionID).FirstOrDefault();
                if (testConnection != null)
                {
                    testConnection.Connect(host, chan, nick, pass);
                }
            }
        }
        public void ircDisconnect(string connectionID)
        {
            lock (Lck)
            {
                var testConnection = this.connections.Where(o => o.connectionID == connectionID).FirstOrDefault();
                if (testConnection != null)
                {
                    testConnection.Disconnect();
                }
            }
        }
        public void ircRaw(string connectionID)
        {

        }
        public void say(string connectionID, string msg)
        {
            lock (Lck)
            {
                var testConnection = this.connections.Where(o => o.connectionID == connectionID).FirstOrDefault();
                if (testConnection != null)
                {
                    testConnection.say(msg);
                }
            }
        }
        #endregion

    }
}
