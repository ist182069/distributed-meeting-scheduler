using MSDAD.Library;
using MSDAD.Server.XML;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MSDAD.Server.Communication
{
    class ServerCommunication
    {
        int server_port;
        string server_ip, server_identifier;        

        ServerLibrary server_library;
        RemoteServer remote_server;
        TcpChannel channel;

        private Dictionary<string, string> client_addresses = new Dictionary<string, string>(); //key = client_identifier; value = client_address

        public ServerCommunication(ServerLibrary server_library)
        {
            this.server_library = server_library;
            this.server_identifier = server_library.ServerIdentifier;
            this.server_port = server_library.ServerPort;
            this.server_ip = server_library.ServerIP;
        }

        public void Start()
        {
            channel = new TcpChannel(this.server_port);
            ChannelServices.RegisterChannel(channel, true);

            this.remote_server = new RemoteServer(this);
            RemotingServices.Marshal(this.remote_server, server_identifier, typeof(RemoteServer));

            LocationAndRoomInit(); // isto vai mudar quando fizermos o AddRoom do PuppetMaster
        }

        public void Create(string meeting_topic, int min_attendees, List<string> slots, List<string> invitees, string client_identifier)
        {
            this.server_library.Create(meeting_topic, min_attendees, slots, invitees, client_identifier);                
            if(invitees == null)
            {
                foreach (KeyValuePair<string, string> address_iter in this.client_addresses)
                {
                    if (address_iter.Key != client_identifier)
                    {
                        ClientInterface client = (ClientInterface)Activator.GetObject(typeof(ClientInterface), "tcp://" + address_iter.Value);
                        client.SendMeeting(meeting_topic, 1, "OPEN");
                    }
                }

            }
            else
            {

                foreach (string invitee_iter in invitees)
                {
                    if (invitee_iter != client_identifier && client_addresses.ContainsKey(invitee_iter))
                    {
                        Console.WriteLine("tcp://" + client_addresses[invitee_iter]);
                        ClientInterface client = (ClientInterface)Activator.GetObject(typeof(ClientInterface), "tcp://" + client_addresses[invitee_iter]);
                        client.SendMeeting(meeting_topic, 1, "OPEN");

                    } else if (invitee_iter == client_identifier)
                    {
                        continue;
                    }
                    else
                    {
                        throw new ServerCoreException(ErrorCodes.NOT_AN_INVITEE);
                    }
                            
                }
            }

            Console.WriteLine("\r\nNew event: " + meeting_topic);
            Console.Write("Please run a command to be run on the server: ");
        }
        public void List(Dictionary<string, string> meeting_query, string client_identifier)
        {
            List<Meeting> event_list = this.server_library.GetEventList();

            ClientInterface remote_client = (ClientInterface)Activator.GetObject(typeof(ClientInterface), "tcp://" + this.client_addresses[client_identifier]);

            foreach (Meeting meeting in event_list)
            {
                if (!meeting_query.ContainsKey(meeting.Topic) && meeting.GetInvitees() == null)
                {
                    remote_client.SendMeeting(meeting.Topic, meeting.Version, meeting.State);
                }
                else if (meeting_query.ContainsKey(meeting.Topic) && !meeting.State.Equals(meeting_query[meeting.Topic]))
                {
                    string state = meeting.State;
                    if (state.Equals("SCHEDULED") && meeting.ClientConfirmed(client_identifier));
                    {
                        string aux = state + "\nClient Confirmed at " + meeting.FinalSlot;
                        remote_client.SendMeeting(meeting.Topic, meeting.Version, aux);
                    }
                    if (!meeting.ClientConfirmed(client_identifier))
                    {
                        remote_client.SendMeeting(meeting.Topic, meeting.Version, meeting.State);
                    }
                }
            } 
        }

        public void Join(string meeting_topic, List<string> slots, string client_identifier)
        {
            this.server_library.Join(meeting_topic, slots, client_identifier);
        }

        public void Close(String meeting_topic, string client_identifier)
        {
            this.server_library.Close(meeting_topic, client_identifier);
        }


        public void BroadcastPing(string message, string client_identifier)
        {
            foreach (KeyValuePair<string, string> address_iter in this.client_addresses)
            {
                if (address_iter.Key != client_identifier)
                {
                    ClientInterface client = (ClientInterface)Activator.GetObject(typeof(ClientInterface), "tcp://" + address_iter.Value);
                    client.Ping(message);
                }
                    
            }             
        }

        public Dictionary<string, string> GetClientAddresses()
        {
            return this.client_addresses;
        }

        public string GetClientAddress(string client_identifier)
        {
            return this.client_addresses[client_identifier];
        }

        public void AddClientAddress(string client_identifier, string client_ip, int client_port)
        {
            string client_address;

            client_address = ServerUtils.AssembleAddress(client_ip, client_port);

            if (ServerUtils.ValidateAddress(client_address))
            {
                lock (this)
                {
                    try
                    {
                        client_addresses.Add(client_identifier, client_address + "/" + client_identifier);
                    }
                    catch (ArgumentException)
                    {
                        throw new ServerCoreException(ErrorCodes.USER_WITH_SAME_ID);
                    }
                }
            }
        }

        public void LocationAndRoomInit()
        {
            string directory_path, file_name;
            string[] directory_files;
            TextReader tr;
            Location location;
            LocationXML locationXML;


            directory_path = ServerUtils.AssembleCurrentPath() + "\\" + "Locations" + "\\";
            directory_files = Directory.GetFiles(directory_path);
            
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(LocationXML));

            lock (this)
            {
                for (int i = 0; i < directory_files.Length; i++)
                {
                    file_name = directory_files[i];
                    tr = new StreamReader(file_name);

                    locationXML = (LocationXML)xmlSerializer.Deserialize(tr);

                    location = new Location(locationXML.Name);                    

                    foreach(RoomXML roomXML in locationXML.RoomViews)
                    {
                        location.Add(new Room(roomXML.Name, roomXML.Capacity));
                    }
                    tr.Close();
                    File.Delete(file_name);
                }
            }
            
            Location lisboa = new Location("Lisboa");
            lisboa.Add(new Room("LisboaA", 10));
            lisboa.Add(new Room("LisboaB", 20));                
            Location coimbra = new Location("Coimbra");
            coimbra.Add(new Room("CoimbraA", 5));
            coimbra.Add(new Room("CoimbraB", 10));
            Location guarda = new Location("Guarda");
            guarda.Add(new Room("GuardaA", 4));
            guarda.Add(new Room("GuardaB", 10));
            Location porto = new Location("Porto");
            porto.Add(new Room("PortoA", 4));
            porto.Add(new Room("PortoB", 6));
            Location braga = new Location("Braga");
            braga.Add(new Room("BragaA", 10));
            braga.Add(new Room("BragaB", 20));

            this.server_library.AddLocation(lisboa);
            this.server_library.AddLocation(coimbra);
            this.server_library.AddLocation(guarda);
            this.server_library.AddLocation(porto);
            this.server_library.AddLocation(braga);
        }

    }
}

