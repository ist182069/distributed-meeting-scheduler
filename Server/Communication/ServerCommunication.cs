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

namespace MSDAD.Server.Communication
{
    class ServerCommunication
    {
        int port;
        string ip, server_identifier;

        ServerLibrary serverLibrary;
        RemoteServer remoteServer;
        TcpChannel channel;

        private Dictionary<string, string> clientAddresses = new Dictionary<string, string>();

        public ServerCommunication(ServerLibrary server_library, string server_identifier, string ip, int port)
        {
            this.serverLibrary = server_library;
            this.server_identifier = server_identifier;
            this.port = port;
            this.ip = ip;
        }

        public void Start()
        {
            channel = new TcpChannel(this.port);
            ChannelServices.RegisterChannel(channel, true);

            this.remoteServer = new RemoteServer(this);
            RemotingServices.Marshal(this.remoteServer, server_identifier, typeof(RemoteServer));

            LocationAndRoomInit(); // isto vai mudar quando fizermos o AddRoom do PuppetMaster
        }

        public void Create(string topic, int minAttendees, List<string> venues, List<string> invitees, string user)
        {
            this.serverLibrary.Create(topic, minAttendees, venues, invitees, user);                
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
                    if(invitee_iter != user && clientAddresses.ContainsKey(invitee_iter))
                    {
                        Console.WriteLine("tcp://" + clientAddresses[invitee_iter]);
                        ClientInterface client = (ClientInterface)Activator.GetObject(typeof(ClientInterface), "tcp://" + clientAddresses[invitee_iter]);
                        client.SendMeeting(topic, 1, "OPEN");

                    } else
                    {
                        throw new ServerCoreException(ErrorCodes.NOT_AN_INVITEE);
                    }
                            
                }
            }

            Console.WriteLine("\r\nNew event: " + topic);
            Console.Write("Please run a command to be run on the server: ");
        }
        public void List(Dictionary<string, string> meetingQuery, string user)
        {
            List<Meeting> eventList = this.serverLibrary.GetEventList();

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
            this.serverLibrary.Join(topic, slots, user);
        }

        public void Close(String topic, string user)
        {
            this.serverLibrary.Close(topic, user);
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

        public Dictionary<string, string> GetClientAddresses()
        {
            return this.clientAddresses;
        }

        public string GetClientAddress(string user)
        {
            return this.clientAddresses[user];
        }

        public void AddClientAddress(string user, string ip, int port)
        {
            string client_address, client_identifier;

            client_address = ServerUtils.AssembleAddress(ip, port);

            if (!clientAddresses.ContainsKey(user) && ServerUtils.ValidateAddress(client_address))
            {
                lock (this)
                {
                    client_identifier = client_address + "/" + user;
                    clientAddresses.Add(user, client_identifier);
                }
            }
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

            this.serverLibrary.AddLocation(Lisboa);
            this.serverLibrary.AddLocation(Coimbra);
            this.serverLibrary.AddLocation(Guarda);
            this.serverLibrary.AddLocation(Porto);
            this.serverLibrary.AddLocation(Braga);
        }

    }
}

