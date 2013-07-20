using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetIrc2;
using System.Threading;
namespace FFIrcRelay
{
    public class IrcWrapper
    {
        public string connectionID { get; set; }
        public IrcClient ircClient { get; set; }
        public dynamic Client { get; set; }
        private string _host { get; set; }
        private string _channel { get; set; }
        private string _nick { get; set; }
        private string _pass { get; set; }


        public IrcWrapper(string cid, dynamic client)
        {
            this.connectionID = cid;
            this.ircClient = new IrcClient();
            this.Client = client;
            this._channel = "";
            this.ircClient.Closed += ircClient_Closed;
            this.ircClient.Connected += ircClient_Connected;
            this.ircClient.GotChatAction += ircClient_GotChatAction;
            this.ircClient.GotJoinChannel += ircClient_GotJoinChannel;
            this.ircClient.GotLeaveChannel += ircClient_GotLeaveChannel;
            this.ircClient.GotMessage += ircClient_GotMessage;
            this.ircClient.GotNotice += ircClient_GotNotice;
            this.ircClient.GotNameListReply += ircClient_GotNameListReply;
            this.ircClient.GotNameListEnd += ircClient_GotNameListEnd;
            this.ircClient.GotChannelTopicChange += ircClient_GotChannelTopicChange;
            this.ircClient.GotNameChange += ircClient_GotNameChange;
            this.ircClient.GotWelcomeMessage += ircClient_GotWelcomeMessage;
            this.ircClient.GotIrcError += ircClient_GotIrcError;
            this.ircClient.GotMotdEnd += ircClient_GotMotdEnd;
            
        }

        void ircClient_GotMotdEnd(object sender, EventArgs e)
        {
            Thread.Sleep(1000 * 1);
            this.ircClient.Join(this._channel);
        }

        void ircClient_GotIrcError(object sender, NetIrc2.Events.IrcErrorEventArgs e)
        {
            Client.onServerMsg(e.Error.ToString());
        }

        void ircClient_GotWelcomeMessage(object sender, NetIrc2.Events.SimpleMessageEventArgs e)
        {
            Client.onServerMsg(e.Message.ToString());
        }

        void ircClient_GotNameChange(object sender, NetIrc2.Events.NameChangeEventArgs e)
        {
            Client.onNick(_channel.ToString(), e.Identity.Nickname.ToString(), e.NewName.ToString());
        }

        void ircClient_GotChannelTopicChange(object sender, NetIrc2.Events.ChannelTopicChangeEventArgs e)
        {
            Client.onTopic(e.Channel.ToString(), e.NewTopic.ToString());
        }

        void ircClient_GotNameListEnd(object sender, NetIrc2.Events.NameListEndEventArgs e)
        {
            Console.WriteLine();
        }

        void ircClient_GotNameListReply(object sender, NetIrc2.Events.NameListReplyEventArgs e)
        {
            List<string> names = new List<string>();
            foreach (var item in e.GetNameList())
            {
                names.Add(item.ToString());
            }
            Client.onNames(e.Channel.ToString(), names.ToArray());
        }

        void ircClient_GotNotice(object sender, NetIrc2.Events.ChatMessageEventArgs e)
        {
            Client.onServerMsg(e.Message.ToString());
        }

        void ircClient_GotMessage(object sender, NetIrc2.Events.ChatMessageEventArgs e)
        {
            if (e.Recipient == _nick)
            {
                Client.onPM(e.Sender.Nickname.ToString(), e.Recipient.ToString(), e.Message.ToString());
            }
            else
            {
                Client.onMessage(e.Sender.Nickname.ToString(), e.Recipient.ToString(), e.Message.ToString());
            }
        }

        void ircClient_GotLeaveChannel(object sender, NetIrc2.Events.JoinLeaveEventArgs e)
        {
            Client.onPart(e.Identity.Nickname.ToString(), " Left the channel..");
        }

        void ircClient_GotJoinChannel(object sender, NetIrc2.Events.JoinLeaveEventArgs e)
        {
            if (e.Identity.Nickname.ToString() == _nick)
            {
                Client.onServerMsg("Now Chatting in " + _channel);
            }
            else
            {
                Client.onJoin(e.Identity.Nickname.ToString(), " Joined " + _channel);
            }
           
        }

        void ircClient_GotChatAction(object sender, NetIrc2.Events.ChatMessageEventArgs e)
        {
            Client.onMEAction(e.Sender.ToString(), e.Message.ToString());
        }

        void ircClient_Connected(object sender, EventArgs e)
        {
            Client.onServerMsg("Connected to Irc Server...");
            this.ircClient.LogIn(_nick, "FireFallUser", _nick, null, null, _pass);
            
        }

        void ircClient_Closed(object sender, EventArgs e)
        {
            Client.onServerMsg("Disconnected to Irc Server...");
        }
        public void Connect(string host, string chan, string nick, string pass)
        {
            if (this.ircClient.IsConnected)
            {
                Disconnect();
            }
            if (pass == string.Empty)
            {
                pass = null;
            }
            this._pass = pass;
            this._nick = nick;
            this._channel = chan;
            IrcClientConnectionOptions opts = new IrcClientConnectionOptions();
           
            this.ircClient.Connect(host);
            
            

        }
        public void Disconnect()
        {
            if (this.ircClient.IsConnected)
            {
                this.ircClient.Close();
            }
            this._channel = "";
        }
        public void Raw()
        {

        }
        public void sendToChannel(string msg)
        {
            if (!string.IsNullOrEmpty(_channel))
            {
                Console.WriteLine("Sending msg to channel " + msg);
                ircClient.Message(_channel, msg);
            }
        }
        public void joinChannel(string chan)
        {
            if (!string.IsNullOrEmpty(_channel))
            {
                ircClient.Leave(_channel);
                _channel = null;
            }
            ircClient.Join(chan);
            _channel = chan;
        }
        public void sendCommand(string command, string args)
        {

            Console.WriteLine("Sending Command {0} {1}", command, args);
            switch (command)
            {
                case "me":
                    ircClient.ChatAction(_channel, args);
                    break;
                case "names":
                    ircClient.IrcCommand("NAMES", _channel);
                    break;
                case "nick":
                    ircClient.ChangeName(args);
                    break;
                case "join":
                    joinChannel(args);
                    break;
                case "leave":
                    if (!String.IsNullOrEmpty(_channel))
                    {
                        ircClient.Leave(_channel);
                        _channel = null;
                    }
                    break;
                default:
                    break;
            }
        }
        public void say(string msg)
        {
                string[] s = msg.Split(' ');
           
                if (s[0].StartsWith("/"))
                {
                    string cmd = s[0].Replace("/", "");
                    string args = "";
                    if (s.Length > 1)
                    {
                        args = string.Join(" ", s.Where((val, idx) => idx != 0).ToArray());
                    }
                    sendCommand(cmd, args);
                }
                else
                {
                    sendToChannel(msg);
                }
            
           
        }
    }
}
