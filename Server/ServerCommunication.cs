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
            List<Location> knownLocations = new List<Location>();
            public void Create(string topic, int minAttendees, List<string> rooms, List<int> invitees, int port)
            {
                Meeting m;
                List<Tuple<Location, DateTime>> parsedSlots = ListOfParsedSlots(rooms);

                lock (this)
                {
                    m = new Meeting(topic, minAttendees, parsedSlots, invitees, port);
                    eventList.Add(m);
                }

                foreach (int p in this.portList)
                {
                    if (p != port & (invitees == null | m.isInvited(port)))
                    {
                        ClientInterface client = (ClientInterface)Activator.GetObject(typeof(ClientInterface), "tcp://localhost:" + p + "/RemoteClient");
                        client.SendMeeting(topic, rooms, port,1, "OPEN");
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
                        client.SendMeeting(mV.Key, m.getSlots(), m.Coordinator, m.getVersion(), m.getState());
                    } 
                }
            }

            public void Join(String topic, List<string> slots, int port)
            {
                Meeting meeting = null;
                try
                {
                    List<Tuple<Location, DateTime>> parsedSlots = ListOfParsedSlots(slots);
                    meeting = GetMeeting(topic);
                    meeting.Apply(parsedSlots, port);                    
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

                LocationAndRoomInit();

                //TO DO predefinir locations e rooms.

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

            public Tuple<Location, DateTime> ParseSlot(String slot)
            {
                Location location = null;
                String[] data = slot.Split(',');
                DateTime date = DateTime.Parse(data[1]);

                foreach (Location l in knownLocations)
                {
                    if (l.Name == data[0])
                    {
                        location = l;
                    }
                }
                if (location == null)
                {
                    throw new ServerCommunicationException(data[0] + " is not a valid location.");
                }

                return new Tuple<Location, DateTime>(location, date);
            }

            public List<Tuple<Location, DateTime>> ListOfParsedSlots(List<string> slots)
            {
                List<Tuple<Location, DateTime>> parsedSlots = new List<Tuple<Location, DateTime>>();
                foreach (string s in slots)
                {
                    parsedSlots.Add(ParseSlot(s));
                }
                return parsedSlots;
            }

            public void LocationAndRoomInit()
            {
                Location Lisboa = new Location("Lisboa");
                Lisboa.AddRoom(new Room("LisboaA", 20));
                Lisboa.AddRoom(new Room("LisboaB", 10));
                Location Coimbra = new Location("Coimbra");
                Coimbra.AddRoom(new Room("CoimbraA", 10));
                Coimbra.AddRoom(new Room("CoimbraB", 5));
                Location Guarda = new Location("Guarda");
                Guarda.AddRoom(new Room("GuardaA", 5));
                Guarda.AddRoom(new Room("GuardaB", 4));
                Location Porto = new Location("Porto");
                Porto.AddRoom(new Room("PortoA", 3));
                Porto.AddRoom(new Room("PortoB", 2));

                knownLocations.Add(Lisboa);
                knownLocations.Add(Coimbra);
                knownLocations.Add(Guarda);
                knownLocations.Add(Porto);
            }

        }
    }
}
