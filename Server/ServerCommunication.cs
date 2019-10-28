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
                Meeting m;
                lock (this)
                {
                    m = new Meeting(topic, minAttendees, rooms, invitees, port);
                    eventList.Add(m);
                }

                foreach (int p in this.portList)
                {
                    if (p != port & (invitees == null | m.isInvited(port)))
                    {
                        ClientInterface client = (ClientInterface)Activator.GetObject(typeof(ClientInterface), "tcp://localhost:" + p + "/RemoteClient");
                        client.SendMeeting(topic, rooms, port,1);
                    }

                }

                Console.WriteLine("\r\nNew event: " + topic);
                Console.Write("Please run a command to be run on the server: ");
            }
            public void List(Dictionary<string, int> meetingQuery, int port)
            {
                foreach(KeyValuePair<string,int> mV in meetingQuery)
                {
                    Meeting m;
                    if ((m=GetMeeting(mV.Key))!=null & m.getVersion() > mV.Value)
                    {
                        ClientInterface client = (ClientInterface)Activator.GetObject(typeof(ClientInterface), "tcp://localhost:" + port + "/RemoteClient");
                        client.SendMeeting(mV.Key, m.getSlots(), m.Coordinator, m.getVersion());
                    } 
                }
            }

            public void Join(String topic, List<string> slots, int port)
            {
                Meeting meeting = null;
                try
                {
                    meeting = GetMeeting(topic);
                    meeting.Apply(slots, port);                    
                } catch (ServerCommunicationException e)
                {
                    throw e;
                } catch (ArgumentException)
                {
                    throw new ServerCommunicationException("You are already a candidate to that meeting.");
                }
            }

            public void Close(String topic, int port)
            {
                foreach (Meeting m in eventList)
                {
                    if (m.Topic == topic && m.Coordinator != port)
                    {
                        throw new ServerCommunicationException("You're not coordinating that meeting.");
                    }
                    if (m.Topic == topic && m.Coordinator == port)
                    {
                        m.Schedule();
                        Console.WriteLine("\r\nEvent Scheduled: " + topic);
                        Console.Write("Please run a command to be run on the server: ");
                        return;
                    }
                }
                throw new ServerCommunicationException("That meeting doesn't seem to exist.");
            }

            private Meeting GetMeeting(string topic)
            {
                foreach(Meeting m in this.eventList)
                {
                    if (m.Topic == topic)
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
                if(!portList.Contains(port))
                {
                    lock (this)
                    {
                        portList.Add(port);
                    }
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
