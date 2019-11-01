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
            ArrayList clientAddresses = new ArrayList();
            List<Meeting> eventList = new List<Meeting>();
            RemoteServer remoteServer;
            TcpChannel channel;
            List<Location> knownLocations = new List<Location>();
            public void Create(string topic, int minAttendees, List<string> rooms, List<string> invitees, string ip, int port)
            {
                string client_address; 
                Meeting m;
                List<Tuple<Location, DateTime>> parsedSlots = ListOfParsedSlots(rooms);

                client_address = ServerUtils.AssembleClientAddress(ip, port);

                lock (this)
                {
                    m = new Meeting(topic, minAttendees, parsedSlots, invitees, client_address);
                    eventList.Add(m);
                }

                
                if(invitees == null)
                {
                    foreach (string address_iter in this.clientAddresses)
                    {
                        if (address_iter != client_address)
                        {
                            ClientInterface client = (ClientInterface)Activator.GetObject(typeof(ClientInterface), "tcp://" + address_iter + "/RemoteClient");
                            client.SendMeeting(topic, rooms, client_address, 1, "OPEN");
                        }

                    }
                } else
                {

                    foreach (string invitee_iter in invitees)
                    {
                        if(ServerUtils.ValidateAddress(invitee_iter) && invitee_iter!=client_address)
                        {
                            Console.WriteLine("tcp://" + invitee_iter + "/RemoteClient");
                            ClientInterface client = (ClientInterface)Activator.GetObject(typeof(ClientInterface), "tcp://" + invitee_iter + "/RemoteClient");
                            client.SendMeeting(topic, rooms, client_address, 1, "OPEN");

                        } else
                        {
                            // TODO throw remote exception
                        }
                            
                    }
                }


                // TODO excepcao caso tentemos adicionar uma data repetida (da para viciar o sistema desta forma)

                Console.WriteLine("\r\nNew event: " + topic);
                Console.Write("Please run a command to be run on the server: ");
            }
            public void List(Dictionary<string, int> meetingQuery, string ip, int port)
            {
                string client_address;

                client_address = ServerUtils.AssembleClientAddress(ip, port);

                foreach(KeyValuePair<string,int> mV in meetingQuery)
                {
                    Meeting m;
                    if ((m=GetMeeting(mV.Key))!=null & m.GetVersion() > mV.Value)
                    {
                        ClientInterface client = (ClientInterface)Activator.GetObject(typeof(ClientInterface), "tcp://" + client_address + "/RemoteClient");
                        client.SendMeeting(mV.Key, m.GetSlots(), m.Coordinator, m.GetVersion(), m.GetState());
                    } 
                }
            }

            public void Join(String topic, List<string> slots, string ip, int port)
            {
                string client_address;
                Meeting meeting = null;
                try
                {
                    client_address = ServerUtils.AssembleClientAddress(ip, port);
                    List<Tuple<Location, DateTime>> parsedSlots = ListOfParsedSlots(slots);
                    meeting = GetMeeting(topic);
                    meeting.Apply(parsedSlots, client_address);                    
                } catch (ServerCommunicationException e)
                {
                    throw e;
                } catch (ArgumentException)
                {
                    throw new ServerCommunicationException("You are already a candidate to that meeting.");
                }
            }

            public void Close(String topic, string ip, int port)
            {
                string client_address;

                client_address = ServerUtils.AssembleClientAddress(ip, port);

                foreach (Meeting m in eventList)
                {
                    if (m.Topic == topic && m.Coordinator != client_address)
                    {
                        throw new ServerCommunicationException("You're not coordinating that meeting.");
                    }
                    if (m.Topic == topic && m.Coordinator == client_address)
                    {
                        int version;
                        bool result = m.Schedule();
                        
                        if(result)
                        {
                            foreach (string address_iter in this.clientAddresses)
                            {
                                version = m.GetVersion();
                                ClientInterface client = (ClientInterface)Activator.GetObject(typeof(ClientInterface), "tcp://" + address_iter + "/RemoteClient");
                                client.SendMeeting(topic, null, client_address, version, "CLOSED");
                            }
                        } else
                        {
                            foreach (string address_iter in this.clientAddresses)
                            {
                                version = m.GetVersion();
                                ClientInterface client = (ClientInterface)Activator.GetObject(typeof(ClientInterface), "tcp://" + address_iter + "/RemoteClient");
                                client.SendMeeting(topic, null, client_address, version, "CANCELED");
                            }
                        }
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
            public void AddClientAddress(string ip, int port)
            {
                string client_address;

                client_address = ServerUtils.AssembleClientAddress(ip, port);

                if(!clientAddresses.Contains(client_address))
                {
                    lock (this)
                    {
                        clientAddresses.Add(client_address);
                    }
                }   
            }
            public void BroadcastPing(string ip, int port, string message)
            {
                string client_address;

                client_address = ServerUtils.AssembleClientAddress(ip, port);

                foreach (string address_iter in this.clientAddresses)
                {
                    if (address_iter != client_address)
                    {
                        ClientInterface client = (ClientInterface)Activator.GetObject(typeof(ClientInterface), "tcp://" + address_iter + "/RemoteClient");
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
