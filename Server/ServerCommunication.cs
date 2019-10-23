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
        class ServerCommunication
        {
            ArrayList portList = new ArrayList();
            List<Meeting> eventList = new List<Meeting>();
            RemoteServer remoteServer;
            TcpChannel channel;
            public void Create(string topic, int minAttendees, List<string> rooms, List<int> invitees, int port)
            {
                lock (this)
                {
                    eventList.Add(new Meeting(topic, minAttendees, rooms,invitees, port));
                }

                Console.WriteLine("New event: " + topic);
            }
            public string List(int port)
            {
                string listData = "";
                foreach(Meeting m in this.eventList)
                {
                    if (m.isInvited(port))
                    {
                        listData += m.getTopic();
                        listData += "\n";
                        listData += m.getSlotsData();
                        listData += "\n";
                    }
                }

                return listData;
            }

            public void Join(String topic, List<string> slots, int port)
            {
                Meeting meeting = null;
                try
                {
                    meeting = GetMeeting(topic);
                    meeting.Join(slots, port);
                } catch (ServerCommunicationException e)
                {
                    throw e;
                }
            }

            private Meeting GetMeeting(string topic)
            {
                foreach(Meeting m in this.eventList)
                {
                    if (m.getTopic() == topic)
                    {
                        return m;
                    }
                }
                throw new ServerCommunicationException("That meeting doesn't seem to exist.");
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
