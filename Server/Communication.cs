using MSDAD.Library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD
{
    namespace Server
    {
        class Communication
        {
            ArrayList portList = new ArrayList();
            List<Meeting> eventList = new List<Meeting>();
            RemoteServer remoteServer;
            TcpChannel channel;
            public void Create(string topic, int minAttendees, List<string> rooms, List<int> invitees)
            {
                lock (this)
                {
                    eventList.Add(new Meeting(topic, minAttendees, rooms));
                }

                if (invitees != null)
                {
                    // Mandar invites
                }

                Console.WriteLine("New event: " + topic);
            }
            public string List()
            {
                string listData = "";
                foreach(Meeting m in this.eventList)
                {
                    listData += m.getTopic();
                    listData += "\n";
                    listData += m.getSlotsData();
                    listData += "\n";
                }

                return listData;
            }
            public void Start(string port)
            {                           
                channel = new TcpChannel(Int32.Parse(port));
                ChannelServices.RegisterChannel(channel, true);
                this.remoteServer = new RemoteServer(this);
                RemotingServices.Marshal(this.remoteServer, "RemoteServer", typeof(RemoteServer));
            }
            public void AddPortArray(int port)
            {
                lock(this)
                {
                    portList.Add(port);
                }
                
            }
            public void BroadcastPing(int port, string message)
            {
                
                foreach (int p in this.portList)
                {
                    if (p != port)
                    {
                        ClientInterface client = (ClientInterface)Activator.GetObject(typeof(ClientInterface), "tcp://localhost:" + p + "/RemoteClient");
                        client.Ping(message);
                    }
                    
                }
             
            }

           
        }
    }
}
