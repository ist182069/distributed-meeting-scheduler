using MSDAD.Library;
using MSDAD.Server.XML;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MSDAD.Server.Communication
{
    class ServerCommunication
    {
        int server_port, tolerated_faults, min_delay, max_delay,crashed_servers = 0,n_replicas;
        string server_ip, server_url, server_identifier, server_remoting;
        List<Boolean> replicas_state = new List<Boolean>();
        object n_replicas_lock = new object();

        ServerLibrary server_library;
        RemoteServer remote_server;
        TcpChannel channel;

        // recebe as mensagens para cada meeting_topic
        private ConcurrentDictionary<string, List<string>> receiving_create = new ConcurrentDictionary<string, List<string>>(); // key: topic ; value: mensagens das replicas
        // topicos a criar que estao pendentes
        private List<string> pending_create = new List<string>();
        private List<string> added_create = new List<string>();

        // Dicionario de acks para cada par reuniao-cliente
        private ConcurrentDictionary<Tuple<string, string>, List<string>> receiving_join = new ConcurrentDictionary<Tuple<string, string>, List<string>>();
        // topicos-cliente que estao pendentes
        private List<Tuple<string, string>> pending_join = new List<Tuple<string, string>>();
        private List<Tuple<string, string>> added_join = new List<Tuple<string, string>>();        

        private ConcurrentDictionary<string, List<string>> receiving_close = new ConcurrentDictionary<string, List<string>>(); // key: topic ; value: mensagens das replicas
        private List<string> pending_close = new List<string>();
        private List<string> added_close = new List<string>();

        private Dictionary<string, string> client_addresses = new Dictionary<string, string>(); //key = client_identifier; value = client_address
        private Dictionary<string, string> server_addresses = new Dictionary<string, string>(); //key = server_identifier; value = server_address        

        private static ConcurrentDictionary<string, object> dictionary_locks = new ConcurrentDictionary<string, object>();

        public delegate void CreateAsyncDelegate(string meeting_topic, int min_attendees, List<string> slots, List<string> invitees, string client_identifier, string server_identifier);
        public delegate void JoinAsyncDelegate(string meeting_topic, List<string> slots, string client_identifier, string server_identifier);
        public delegate void CloseAsyncDelegate(string meeting_topic, string client_identifier, string server_identifier);

        public static void CreateAsyncCallBack(IAsyncResult ar)
        {
            CreateAsyncDelegate del = (CreateAsyncDelegate)((AsyncResult)ar).AsyncDelegate;
            return;
        }

        public static void JoinAsyncCallBack(IAsyncResult ar)
        {
            JoinAsyncDelegate del = (JoinAsyncDelegate)((AsyncResult)ar).AsyncDelegate;
            return;
        }

        public static void CloseAsyncCallBack(IAsyncResult ar)
        {
            CloseAsyncDelegate del = (CloseAsyncDelegate)((AsyncResult)ar).AsyncDelegate;
            return;
        }

        public ServerCommunication(ServerLibrary server_library)
        {
            this.server_library = server_library;
            this.server_identifier = server_library.ServerIdentifier;
            this.server_port = server_library.ServerPort;
            this.server_ip = server_library.ServerIP;
            this.server_remoting = server_library.ServerRemoting;
            this.tolerated_faults = server_library.ToleratedFaults;
            this.min_delay = server_library.MinDelay;
            this.max_delay = server_library.MaxDelay;
        }

        public void Start()
        {
            channel = new TcpChannel(this.server_port);
            ChannelServices.RegisterChannel(channel, true);

            this.remote_server = new RemoteServer(this);
            RemotingServices.Marshal(this.remote_server, server_remoting, typeof(RemoteServer));

            LocationAndRoomInit();
            ServerURLInit();

            this.server_url = ServerUtils.AssembleRemotingURL(this.server_ip, this.server_port, this.server_remoting);
            n_replicas = this.server_port - 3000;
            for (int i = 0; i < 101; i++)
            {
                this.replicas_state.Add(true);
            }

            int server_iter = 1;

            foreach (string replica_url in this.server_addresses.Values)
            {
                if (server_iter > n_replicas)
                {
                    break;
                }

                try
                {
                    ServerInterface remote_server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), replica_url);
                    remote_server.NReplicasUpdate(n_replicas);
                }
                catch (System.Net.Sockets.SocketException se)
                {
                    Console.WriteLine("Deteta Crash no Start\n");
                    replicas_state[ServerUtils.GetPortFromUrl(replica_url) - 3000] = false;
                    this.crashed_servers++;
                }

                server_iter++;
            }
        }

        public void Create(string meeting_topic, int min_attendees, List<string> slots, List<string> invitees, string client_identifier, string create_replica_identifier)
        {
            object create;

            if (!dictionary_locks.ContainsKey(meeting_topic))
            {
                create = new object();
                dictionary_locks.TryAdd(meeting_topic, create);
            }
            else
            {
                create = dictionary_locks[meeting_topic];
            }
       
            if (!added_create.Contains(meeting_topic))
            {

                if (!this.receiving_create.ContainsKey(meeting_topic))
                {
                    List<string> received_messages = new List<string>();
                    received_messages.Add(this.server_identifier);
                    this.receiving_create.AddOrUpdate(meeting_topic, received_messages, (key, oldValue) => received_messages);
                }
                else
                {
                    List<string> received_messages = this.receiving_create[meeting_topic];

                    if (!received_messages.Contains(create_replica_identifier))
                    {
                        received_messages.Add(create_replica_identifier);
                        this.receiving_create[meeting_topic] = received_messages;
                    }
                }

                lock (create)
                {
                    if (!pending_create.Contains(meeting_topic))
                    {
                        pending_create.Add(meeting_topic);

                        int server_iter = 1;
                        TimeSpan timeout = new TimeSpan(0, 0, 0, 10, 0); //TODO: AJUSTAR!

                        foreach (string replica_url in this.server_addresses.Values)
                        {
                            if (server_iter > this.n_replicas)
                            {
                                break;
                            }

                            if (this.replicas_state[server_iter] == false)
                            {
                                server_iter++;
                                continue;
                            }

                            if (!replica_url.Equals(this.server_url))
                            {                               
                                try
                                {
                                    ServerInterface remote_server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), replica_url);
                                    remote_server.IsAlive();

                                    CreateAsyncDelegate RemoteDel = new CreateAsyncDelegate(remote_server.Create);
                                    AsyncCallback RemoteCallback = new AsyncCallback(ServerCommunication.CreateAsyncCallBack);
                                    Thread thread = new Thread(() =>
                                    {
                                        IAsyncResult RemAr = RemoteDel.BeginInvoke(meeting_topic, min_attendees, slots, invitees, client_identifier, this.server_identifier, RemoteCallback, null);
                                    });

                                    thread.Start();

                                    bool finished = thread.Join(timeout);
                                    if (!finished) {
                                        this.replicas_state[server_iter] = false;
                                        this.crashed_servers++;
                                        thread.Abort();
                                    }
                                    
                                }
                                catch (System.Net.Sockets.SocketException se)
                                {
                                    Console.WriteLine("entrei excecao create: " + this.replicas_state[n_replicas]);
                                    this.replicas_state[server_iter] = false;
                                    this.crashed_servers++;
                                    Console.WriteLine("sai excecao create: " + this.replicas_state[n_replicas]);
                                }
                            }

                            server_iter++;
                        }

                        // TODO:  Por timer
                        int timer_counter = 0;
                        while (timer_counter<40)
                        {
                            Thread.Sleep(250);
                            float current_messages = (float)this.receiving_create[meeting_topic].Count;

                            if (current_messages > (float)(this.n_replicas-this.crashed_servers) / 2)
                            {
                                this.server_library.Create(meeting_topic, min_attendees, slots, invitees, client_identifier);
                                this.added_create.Add(meeting_topic);
                                if (invitees == null)
                                {
                                    foreach (KeyValuePair<string, string> address_iter in this.client_addresses)
                                    {
                                        if (address_iter.Key != client_identifier)
                                        {
                                            ClientInterface client = (ClientInterface)Activator.GetObject(typeof(ClientInterface), "tcp://" + address_iter.Value);
                                            client.SendMeeting(meeting_topic, 1, "OPEN", null);
                                        }
                                    }
                                }

                                Console.WriteLine("\r\nNew event: " + meeting_topic);
                                Console.Write("Please run a command to be run on the server: ");
                                break;
                            }
                            timer_counter++;
                        }

                        if(timer_counter == 40)
                        {
                            //Executa o create outra vez
                            Console.WriteLine("Kabum");
                        }
                    }                    
                }                
            }            
        }
        public void List(Dictionary<string, string> meeting_query, string client_identifier)
        {
            List<Meeting> event_list = this.server_library.GetEventList();

            ClientInterface remote_client = (ClientInterface)Activator.GetObject(typeof(ClientInterface), "tcp://" + this.client_addresses[client_identifier]);

            foreach (Meeting meeting in event_list)
            {
                if (!meeting_query.ContainsKey(meeting.Topic) && meeting.GetInvitees() == null)
                {
                    string state = meeting.State;
                    if (state.Equals("SCHEDULED") && meeting.ClientConfirmed(client_identifier))
                    {
                        string extraInfo = "Client Confirmed at " + meeting.FinalSlot;
                        remote_client.SendMeeting(meeting.Topic, meeting.Version, meeting.State, extraInfo);
                    }
                    else
                    {
                        remote_client.SendMeeting(meeting.Topic, meeting.Version, meeting.State, null);
                    }
                }
                else if (!meeting_query.ContainsKey(meeting.Topic) && meeting.GetInvitees() != null)
                {
                    if (meeting.GetInvitees().Contains(client_identifier)) {
                        string state = meeting.State;
                        if (state.Equals("SCHEDULED") && meeting.ClientConfirmed(client_identifier))
                        {
                            string extraInfo = "Client Confirmed at " + meeting.FinalSlot;
                            remote_client.SendMeeting(meeting.Topic, meeting.Version, meeting.State, extraInfo);
                        }
                        else
                        {
                            remote_client.SendMeeting(meeting.Topic, meeting.Version, meeting.State, null);
                        }
                    }
                }
                else if (meeting_query.ContainsKey(meeting.Topic) && !meeting.State.Equals(meeting_query[meeting.Topic]))
                {
                    string state = meeting.State;
                    if (state.Equals("SCHEDULED") && meeting.ClientConfirmed(client_identifier))
                    {
                        string extraInfo = "Client Confirmed at " + meeting.FinalSlot;
                        remote_client.SendMeeting(meeting.Topic, meeting.Version, meeting.State, extraInfo);
                    }
                    else
                    {
                        remote_client.SendMeeting(meeting.Topic, meeting.Version, meeting.State, null);
                    }
                }
            }
        }

        public void Join(string meeting_topic, List<string> slots, string client_identifier, string join_server_identifier)
        {
            object join;
            Tuple<string, string> join_tuple;
            
            join_tuple = new Tuple<string, string>(meeting_topic, client_identifier);

            if (!dictionary_locks.ContainsKey(meeting_topic))
            {
                join = new object();
                dictionary_locks.TryAdd(meeting_topic, join);
            }
            else
            {
                join = dictionary_locks[meeting_topic];
            }

            if (!this.added_join.Contains(join_tuple))
            {

                if (!this.receiving_join.ContainsKey(join_tuple))
                {
                    List<string> received_messages = new List<string>();
                    received_messages.Add(this.server_identifier);
                    this.receiving_join.AddOrUpdate(join_tuple, received_messages, (key, oldValue) => received_messages);
                }
                else
                {
                    List<string> received_messages = this.receiving_join[join_tuple];

                    if (!received_messages.Contains(join_server_identifier))
                    {
                        received_messages.Add(join_server_identifier);
                        this.receiving_join[join_tuple] = received_messages;
                    }
                }

                lock(join)
                {
                    if (!this.pending_join.Contains(join_tuple))
                    {
                        this.pending_join.Add(join_tuple);

                        int server_iter = 1;
                        TimeSpan timeout = new TimeSpan(0, 0, 0, 10, 0); //TODO: AJUSTAR!

                        foreach (string replica_url in this.server_addresses.Values)
                        {
                            if (server_iter > this.n_replicas)
                            {
                                break;
                            }

                            if (this.replicas_state[server_iter] == false)
                            {
                                server_iter++;
                                continue;
                            }

                            if (!replica_url.Equals(this.server_url))
                            {
                                try
                                {
                                    ServerInterface remote_server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), replica_url);
                                    remote_server.IsAlive();
                                    JoinAsyncDelegate RemoteDel = new JoinAsyncDelegate(remote_server.Join);
                                    AsyncCallback RemoteCallback = new AsyncCallback(ServerCommunication.JoinAsyncCallBack);
                                    Thread thread = new Thread(() =>
                                    {
                                        IAsyncResult RemAr = RemoteDel.BeginInvoke(meeting_topic, slots, client_identifier, this.server_identifier, RemoteCallback, null);
                                    });

                                    thread.Start();

                                    bool finished = thread.Join(timeout);
                                    if (!finished) {
                                        this.replicas_state[server_iter] = false;
                                        this.crashed_servers++;
                                        thread.Abort();
                                    }
                                }
                                catch (System.Net.Sockets.SocketException se)
                                {
                                    this.replicas_state[server_iter] = false;
                                    this.crashed_servers++;
                                }
                            }

                            server_iter++;
                        }
                        
                        // TODO:  Por timer
                        while (true)
                        {
                            float current_messages = (float)this.receiving_join[join_tuple].Count;

                            if (current_messages > (float)(this.n_replicas-this.crashed_servers)/ 2)
                            {
                                this.server_library.Join(meeting_topic, slots, client_identifier);
                                this.added_join.Add(join_tuple);
                                break;
                            }
                        }
                    }                    
                }                
            }
        }

        public void Close(string meeting_topic, string client_identifier, string close_replica_identifier)
        {
            object close;

            if (!dictionary_locks.ContainsKey(meeting_topic))
            {
                close = new object();
                dictionary_locks.TryAdd(meeting_topic, close);
            }
            else
            {
                close = dictionary_locks[meeting_topic];
            }

            if(!added_close.Contains(meeting_topic))
            {
                if (!this.receiving_close.ContainsKey(meeting_topic))
                {
                    List<string> received_messages = new List<string>();
                    received_messages.Add(this.server_identifier);
                    this.receiving_close.AddOrUpdate(meeting_topic, received_messages, (key, oldValue) => received_messages);
                }
                else
                {
                    List<string> received_messages = this.receiving_close[meeting_topic];

                    if (!received_messages.Contains(close_replica_identifier))
                    {
                        received_messages.Add(close_replica_identifier);
                        this.receiving_close[meeting_topic] = received_messages;
                    }
                }

                lock (close)
                {
                    if (!this.pending_close.Contains(meeting_topic))
                    {
                        this.pending_close.Add(meeting_topic);

                        int server_iter = 1;
                        TimeSpan timeout = new TimeSpan(0, 0, 0, 10, 0); //TODO: AJUSTAR!

                        foreach (string replica_url in this.server_addresses.Values)
                        {
                            if (server_iter > this.n_replicas)
                            {
                                break;
                            }

                            if(this.replicas_state[server_iter] == false)
                            {
                                server_iter++;
                                continue;
                            }

                            if (!replica_url.Equals(this.server_url))
                            {
                                try
                                {
                                    ServerInterface remote_server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), replica_url);
                                    remote_server.IsAlive();
                                    CloseAsyncDelegate RemoteDel = new CloseAsyncDelegate(remote_server.Close);
                                    AsyncCallback RemoteCallback = new AsyncCallback(ServerCommunication.CloseAsyncCallBack);
                                    Thread thread = new Thread(() =>
                                    {
                                        IAsyncResult RemAr = RemoteDel.BeginInvoke(meeting_topic, client_identifier, this.server_identifier, RemoteCallback, null);
                                    });

                                    thread.Start();

                                    bool finished = thread.Join(timeout);
                                    if (!finished)
                                    {
                                        this.replicas_state[server_iter] = false;
                                        this.crashed_servers++;
                                        thread.Abort();
                                    }
                                }
                                catch (System.Net.Sockets.SocketException se)
                                {
                                    this.replicas_state[server_iter] = false;
                                    this.crashed_servers++;
                                }
                            }

                            server_iter++;
                        }

                        // TODO:  Por timer
                        while (true)
                        {
                            float current_messages = (float)this.receiving_close[meeting_topic].Count;

                            if (current_messages > (float)(this.n_replicas-this.crashed_servers)/ 2)
                            {
                                this.server_library.Close(meeting_topic, client_identifier);
                                this.added_close.Add(meeting_topic);
                                break;
                            }
                        }
                    }                    
                }                
            }                        
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

        public void AddClientAddress(string client_identifier, string client_remoting, string client_ip, int client_port)
        {
            string client_address;

            client_address = ServerUtils.AssembleAddress(client_ip, client_port);

            if (ServerUtils.ValidateAddress(client_address))
            {
                lock (this)
                {
                    try
                    {
                        client_addresses.Add(client_identifier, client_address + "/" + client_remoting);
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

                    foreach (RoomXML roomXML in locationXML.RoomViews)
                    {
                        location.Add(new Room(roomXML.Name, roomXML.Capacity));
                    }
                    tr.Close();
                    this.server_library.AddLocation(location);
                }
            }
        }

        public void ServerURLInit()
        {
            string server_id, server_url;

            for(int i = 1; i < 10; i++)
            {
                server_id = "s" + i;
                server_url = "tcp://localhost:300" + i + "/server" + i;
                this.server_addresses.Add(server_id, server_url);
            }

            for(int i = 10; i < 100; i++)
            {
                server_id = "s" + i;
                server_url = "tcp://localhost:30" + i + "/server" + i;
                this.server_addresses.Add(server_id, server_url);
            }
            
        }

        public void Status()
        {
            this.server_library.Status();
        }

        public int Delay()
        {
            int delay;

            Random r = new Random();
            delay = r.Next(this.min_delay, max_delay);

            return delay;
        }       

        public void setNReplica(int n_replicas)
        {
            lock (n_replicas_lock)
            {
                if (this.n_replicas < n_replicas)
                {
                    Console.WriteLine("previous: " + this.n_replicas + ", next: " + n_replicas);
                    this.n_replicas = n_replicas;
                }
            }
            
        }

    }
}

