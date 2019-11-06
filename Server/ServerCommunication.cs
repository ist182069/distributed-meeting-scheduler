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

namespace MSDAD.Server
{
    class ServerCommunication
    {
        Dictionary<string, string> clientAddresses = new Dictionary<string, string>();
        List<Meeting> eventList = new List<Meeting>();
        RemoteServer remoteServer;
        TcpChannel channel;
        List<Location> knownLocations = new List<Location>();
        public void Create(string topic, int minAttendees, List<string> venues, List<string> invitees, string user)
        {
            string client_address; 
            Meeting m;
            List<Tuple<Location, DateTime>> parsedSlots = ListOfParsedSlots(venues);

            lock (this)
            {
                m = new Meeting(topic, minAttendees, parsedSlots, invitees, user);
                eventList.Add(m);
            }

                
            if(invitees == null)
            {
                foreach (KeyValuePair<string, string> address_iter in this.clientAddresses)
                {
                    if (address_iter.Key != user)
                    {
                        ClientInterface client = (ClientInterface)Activator.GetObject(typeof(ClientInterface), "tcp://" + address_iter.Value);
                        client.SendMeeting(topic, 1, "OPEN");
                    }
                }

            }
            else
            {

                foreach (string invitee_iter in invitees)
                {
                    if(invitee_iter != user)
                    {
                        Console.WriteLine("tcp://" + this.clientAddresses[invitee_iter]);
                        ClientInterface client = (ClientInterface)Activator.GetObject(typeof(ClientInterface), "tcp://" + this.clientAddresses[invitee_iter]);
                        client.SendMeeting(topic, 1, "OPEN");

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
        public void List(Dictionary<string, string> meetingQuery, string user)
        {
            ClientInterface client = (ClientInterface)Activator.GetObject(typeof(ClientInterface), "tcp://" + this.clientAddresses[user]);

            foreach (Meeting meeting in eventList)
            {
                if (!meetingQuery.ContainsKey(meeting.Topic) && meeting.GetInvitees() == null)
                {
                    client.SendMeeting(meeting.Topic, meeting.GetVersion(), meeting.GetState());
                }
                else if (meetingQuery.ContainsKey(meeting.Topic) && !meeting.GetState().Equals(meetingQuery[meeting.Topic]))
                {
                    string state = meeting.GetState();
                    if (state.Equals("SCHEDULED") && meeting.ClientConfirmed(user));
                    {
                        string aux = state + "\nClient Confirmed at " + meeting.GetFinalSlot();
                        client.SendMeeting(meeting.Topic, meeting.GetVersion(), aux);
                    }
                    if (!meeting.ClientConfirmed(user))
                    {
                        client.SendMeeting(meeting.Topic, meeting.GetVersion(), meeting.GetState());
                    }
                }
            } 
        }

        public void Join(string topic, List<string> slots, string user)
        {
            Meeting meeting = null;
            try
            {
                List<Tuple<Location, DateTime>> parsedSlots = ListOfParsedSlots(slots);
                meeting = GetMeeting(topic);
                meeting.Apply(parsedSlots, user);
            }
            catch (ServerCoreException sce)
            {
                throw sce;
            }
        }

        public void Close(String topic, string user)
        {
            GetMeeting(topic).Schedule(user);
            Console.Write("Please run a command to be run on the server: ");
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
            throw new ServerCoreException(ErrorCodes.NONEXISTENT_MEETING);
        }
        public void Start(string port)
        {                           
            channel = new TcpChannel(Int32.Parse(port));
            ChannelServices.RegisterChannel(channel, true);
            this.remoteServer = new RemoteServer(this);
            RemotingServices.Marshal(this.remoteServer, "RemoteServer", typeof(RemoteServer));

            LocationAndRoomInit();

        }
        public void AddClientAddress(string user, string ip, int port)
        {
            string client_address, client_identifier;

            client_address = ServerUtils.AssembleClientAddress(ip, port);

            if(!clientAddresses.ContainsKey(user) && ServerUtils.ValidateAddress(client_address))
            {
                lock (this)
                {
                    client_identifier = client_address + "/" + user;
                    clientAddresses.Add(user, client_identifier);
                }
            }   
        }
        public void BroadcastPing(string message, string user)
        {
            foreach (KeyValuePair<string, string> address_iter in this.clientAddresses)
            {
                if (address_iter.Key != user)
                {
                    ClientInterface client = (ClientInterface)Activator.GetObject(typeof(ClientInterface), "tcp://" + address_iter.Value);
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
                throw new ServerCoreException(ErrorCodes.NOT_A_LOCATION);
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
            Lisboa.Add(new Room("LisboaA", 10));
            Lisboa.Add(new Room("LisboaB", 20));                
            Location Coimbra = new Location("Coimbra");
            Coimbra.Add(new Room("CoimbraA", 5));
            Coimbra.Add(new Room("CoimbraB", 10));
            Location Guarda = new Location("Guarda");
            Guarda.Add(new Room("GuardaA", 4));
            Guarda.Add(new Room("GuardaB", 10));
            Location Porto = new Location("Porto");
            Porto.Add(new Room("PortoA", 4));
            Porto.Add(new Room("PortoB", 6));
            Location Braga = new Location("Braga");
            Braga.Add(new Room("BragaA", 10));
            Braga.Add(new Room("BragaB", 20));

            knownLocations.Add(Lisboa);
            knownLocations.Add(Coimbra);
            knownLocations.Add(Guarda);
            knownLocations.Add(Porto);
            knownLocations.Add(Braga);
        }

    }
}

