using MSDAD.Library;
using MSDAD.Server.Logs;
using MSDAD.Server.XML;
using Newtonsoft.Json;
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
        int server_port, tolerated_faults, min_delay, max_delay, n_replicas;
        string server_ip, server_url, server_identifier, server_remoting;        

        ServerLibrary server_library;
        RemoteServer remote_server;
        TcpChannel channel;

        private ConcurrentDictionary<string, List<string>> logs_dictionary = new ConcurrentDictionary<string, List<string>>();

        private ConcurrentDictionary<string, List<string>> atomic_read_received = new ConcurrentDictionary<string, List<string>>(); // key: topic ; value: id da replica que respondeu
        private ConcurrentDictionary<string, List<Tuple<int, List<string>>>> atomic_read_tuples = new ConcurrentDictionary<string, List<Tuple<int, List<string>>>>(); // key: topic+uid ; value: Lista com os (topic, version, state) de cada cliente

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
        
        public delegate void CreateAsyncDelegate(string meeting_topic, int min_attendees, List<string> slots, List<string> invitees, string client_identifier, string server_identifier, int hops, List<string> logs_list, int sent_version);
        public delegate void JoinAsyncDelegate(string meeting_topic, List<string> slots, string client_identifier, string server_identifier, int hops, List<string> logs_list, int sent_version);
        public delegate void CloseAsyncDelegate(string meeting_topic, string client_identifier, string server_identifier, int hops, List<string> logs_list, int sent_version);

        public delegate void GetMeetingFromServerAsyncDelegate(string meeting_topic, string server_identifier);
        public delegate void SendMeetingToServerAsyncDelegate(string meeting_topic, int version, List<string> logs_list, string server_identifier);

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

        public static void GetMeetingFromServerAsyncCallBack(IAsyncResult ar)
        {
            GetMeetingFromServerAsyncDelegate del = (GetMeetingFromServerAsyncDelegate)((AsyncResult)ar).AsyncDelegate;
            return;
        }

        public static void SendMeetingToServerAsyncCallBack(IAsyncResult ar)
        {
            SendMeetingToServerAsyncDelegate del = (SendMeetingToServerAsyncDelegate)((AsyncResult)ar).AsyncDelegate;
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
            n_replicas = (tolerated_faults * 2) + 1;          
        }

        public void Create(string meeting_topic, int min_attendees, List<string> slots, List<string> invitees, string client_identifier, string create_replica_identifier, int hops, List<string> logs_list, int sent_version)
        {
            object create;
            Tuple<string, string> join_tuple;

            join_tuple = new Tuple<string, string>(meeting_topic, client_identifier);

            if (!dictionary_locks.ContainsKey(meeting_topic))
            {
                create = new object();
                dictionary_locks.TryAdd(meeting_topic, create);
            }
            else
            {
                create = dictionary_locks[meeting_topic];
            }            

            if (!added_create.Contains(meeting_topic) && !this.added_join.Contains(join_tuple) && !added_close.Contains(meeting_topic))
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
                    int written_version;
                    Console.WriteLine("entrou create lock");
                    if (hops == 0)
                    {
                        Tuple<bool, List<string>> atomic_read_tuple = this.AtomicRead(meeting_topic);
                        bool atomic_read_result = atomic_read_tuple.Item1;
                        List<string> highest_value_list;

                        if (atomic_read_tuple.Item2 != null)
                        {
                            highest_value_list = atomic_read_tuple.Item2;
                        }
                        else
                        {
                            highest_value_list = new List<string>();
                        }

                        if (atomic_read_result)
                        {
                            Console.WriteLine("entrou if");
                            written_version = this.server_library.WriteMeeting(meeting_topic, highest_value_list);
                            hops++;
                            written_version++;
                            Console.WriteLine("");
                            Console.WriteLine(written_version);
                            Console.WriteLine("");
                            if(written_version<0)
                            {
                                written_version = 1;
                            }
                            this.CreateBroadcast(meeting_topic, min_attendees, slots, invitees, client_identifier, hops, highest_value_list, written_version);
                            Console.WriteLine("entrou executou");
                        }
                    }
                    else
                    {

                        Console.WriteLine("entrou no else");
                        this.AtomicWrite(meeting_topic, logs_list);
                        this.CreateBroadcast(meeting_topic, min_attendees, slots, invitees, client_identifier, hops, logs_list, sent_version);
                        Console.WriteLine("entrou executou");
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
                if (!meeting_query.ContainsKey(meeting.Topic) && meeting.Invitees == null)
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
                else if (!meeting_query.ContainsKey(meeting.Topic) && meeting.Invitees != null)
                {
                    if (meeting.Invitees.Contains(client_identifier)) {
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

        public void Join(string meeting_topic, List<string> slots, string client_identifier, string join_server_identifier, int hops, List<string> logs_list, int sent_version)
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

            if (!this.added_join.Contains(join_tuple) && !this.added_close.Contains(meeting_topic))
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
                    int written_version;
                    Console.WriteLine("entrou join lock");
                    if(hops==0)
                    {
                        Tuple<bool, List<string>> atomic_read_tuple = this.AtomicRead(meeting_topic);
                        bool atomic_read_result = atomic_read_tuple.Item1;
                        List<string> highest_value_list;

                        if (atomic_read_tuple.Item2!=null)
                        {
                            highest_value_list = atomic_read_tuple.Item2;
                        }
                        else
                        {
                            highest_value_list = new List<string>();
                        }

                        if (atomic_read_result)
                        {
                            Console.WriteLine("entrou if");
                            written_version = this.server_library.WriteMeeting(meeting_topic, highest_value_list);                            
                            hops++;
                            written_version++;
                            Console.WriteLine("");
                            Console.WriteLine(written_version);
                            Console.WriteLine("");
                            this.JoinBroadcast(meeting_topic, slots, client_identifier, hops, join_tuple, highest_value_list, written_version);
                            Console.WriteLine("entrou executou");
                        }
                    }
                    else
                    {
                        Console.WriteLine("entrou no else");
                        this.AtomicWrite(meeting_topic, logs_list);
                        this.JoinBroadcast(meeting_topic, slots, client_identifier, hops, join_tuple, logs_list, sent_version);
                        Console.WriteLine("entrou executou");
                    }
              
                }                
            }
        }       
        public void Close(string meeting_topic, string client_identifier, string close_replica_identifier, int hops, List<string> logs_list, int sent_version)
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
                    int written_version;
                    Console.WriteLine("entrou close lock");
                    this.CloseBroadcast(meeting_topic, client_identifier, hops, logs_list, sent_version);

                    if (hops == 0)
                    {
                        Tuple<bool, List<string>> atomic_read_tuple = this.AtomicRead(meeting_topic);
                        bool atomic_read_result = atomic_read_tuple.Item1;
                        List<string> highest_value_list;

                        if (atomic_read_tuple.Item2 != null)
                        {
                            highest_value_list = atomic_read_tuple.Item2;
                        }
                        else
                        {
                            highest_value_list = new List<string>();
                        }

                        if (atomic_read_result)
                        {
                            Console.WriteLine("entrou if");
                            written_version = this.server_library.WriteMeeting(meeting_topic, highest_value_list);
                            hops++;
                            written_version++;
                            Console.WriteLine("");
                            Console.WriteLine(written_version);
                            Console.WriteLine("");
                            this.CloseBroadcast(meeting_topic, client_identifier, hops, highest_value_list, written_version);
                            Console.WriteLine("close executou");
                            
                            List<string> dred = logs_dictionary[meeting_topic];

                            Console.WriteLine("dred");
                            foreach (string d in dred)
                            {
                                Console.WriteLine(d);
                            }
                            Console.WriteLine("close executou");

                        }
                    }
                    else
                    {
                        Console.WriteLine("entrou no else");
                        this.AtomicWrite(meeting_topic, logs_list);
                        this.CloseBroadcast(meeting_topic, client_identifier, hops, logs_list, sent_version);
                        Console.WriteLine("close executou");
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

        public void GetMeeting(string meeting_topic, string replica_identifier)
        {
            int version;
            string replica_url;            
            List<string> list = new List<string>();

            version = this.server_library.GetVersion(meeting_topic);
            replica_url = server_addresses[replica_identifier];

            if(this.logs_dictionary.ContainsKey(meeting_topic))
            {
                list = this.logs_dictionary[meeting_topic];
            }
            

            ServerInterface remote_server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), replica_url);

            try
            {
                SendMeetingToServerAsyncDelegate RemoteDel = new SendMeetingToServerAsyncDelegate(remote_server.SendMeeting);
                AsyncCallback RemoteCallback = new AsyncCallback(ServerCommunication.SendMeetingToServerAsyncCallBack);
                IAsyncResult RemAr = RemoteDel.BeginInvoke(meeting_topic, version, list, this.server_identifier, RemoteCallback, null);
                Console.WriteLine("Enviou GM: " + replica_url);
            }
            catch (System.Net.Sockets.SocketException se)
            {
                Console.WriteLine(se.Message);
            }
        }

        public void SendMeeting(string meeting_topic, int version, List<string> logs_list, string replica_identifier)
        {
            List<string> received_messages = this.atomic_read_received[meeting_topic];
            List<Tuple<int, List<string>>> received_tuples = this.atomic_read_tuples[meeting_topic];

            if (!received_messages.Contains(replica_identifier))
            {
                received_messages.Add(replica_identifier);
                this.atomic_read_received[meeting_topic] = received_messages;

                received_tuples.Add(new Tuple<int, List<string>>(version, logs_list));
                this.atomic_read_tuples[meeting_topic] = received_tuples;
                Console.WriteLine("passou tudo: ");
            }
        }

        private Tuple<bool, List<string>> AtomicRead(string meeting_topic)
        {
            List<string> highest_value_list = null;

            bool result = false;
            List<string> received_messages = new List<string>();
            received_messages.Add(this.server_identifier);            
            int version = this.server_library.GetVersion(meeting_topic);
            List<Tuple<int, List<string>>> received_versions = new List<Tuple<int, List<string>>>();
            this.atomic_read_received.AddOrUpdate(meeting_topic, received_messages, (key, oldValue) => received_messages);
            this.atomic_read_tuples.AddOrUpdate(meeting_topic, received_versions, (key, oldValue) => received_versions);

            int server_iter = 1;

            Console.WriteLine("entrou atomic read");
            foreach (string replica_url in this.server_addresses.Values)
            {
                if (server_iter > n_replicas)
                {
                    break;
                }

                if (!replica_url.Equals(this.server_url))
                {
                    ServerInterface remote_server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), replica_url);
                    try
                    {
                        GetMeetingFromServerAsyncDelegate RemoteDel = new GetMeetingFromServerAsyncDelegate(remote_server.GetMeeting);
                        AsyncCallback RemoteCallback = new AsyncCallback(ServerCommunication.GetMeetingFromServerAsyncCallBack);
                        IAsyncResult RemAr = RemoteDel.BeginInvoke(meeting_topic, this.server_identifier, RemoteCallback, null);
                        Console.WriteLine("enviou para os gajos");
                    }
                    catch (System.Net.Sockets.SocketException se)
                    {
                        Console.WriteLine(se.Message);
                    }
                }

                server_iter++;
            }

            while (true)
            {
                float current_messages = (float)this.atomic_read_received[meeting_topic].Count;
                Thread.Sleep(3000);
                Console.WriteLine(current_messages);
                if (current_messages > (float)n_replicas / 2)
                {
                    Tuple<int, List<string>> highest_version_tuple = this.ReadHighestVersion(meeting_topic);
                    highest_value_list = highest_version_tuple.Item2;                    
                    Console.WriteLine("!!!Fez Atomic Read!!!");
                    result = true;
                    break;
                }
            }

            return new Tuple<bool, List<string>>(result, highest_value_list);
        }
        public void AtomicWrite(string meeting_topic, List<string> logs_list)
        {
            this.server_library.WriteMeeting(meeting_topic, logs_list);
            Console.WriteLine("!!!Fez Atomic Write!!!");
        }

        private Tuple<int, List<string>> ReadHighestVersion(string meeting_topic)
        {
            int current_version, maximum_version = Int32.MinValue;
            Tuple<int, List<string>> highest_version_tuple = null;
            List<Tuple<int, List<string>>> received_tuples = this.atomic_read_tuples[meeting_topic];

            foreach(Tuple<int, List<string>> current_tuple in received_tuples)
            {
                current_version = current_tuple.Item1;

                if(current_version>maximum_version)
                {
                    maximum_version = current_version;
                    highest_version_tuple = current_tuple;
                }
            }

            return highest_version_tuple;
        }

        private void CreateBroadcast(string meeting_topic, int min_attendees, List<string> slots, List<string> invitees, string client_identifier, int hops, List<string> logs_list, int sent_version)
        {
            Console.WriteLine("estado:" + this.server_library.GetVersion(meeting_topic) + " " + sent_version);
            if (!pending_create.Contains(meeting_topic) && this.server_library.GetVersion(meeting_topic) < sent_version)
            {
                Console.WriteLine("if");
                Console.WriteLine();
                pending_create.Add(meeting_topic);

                int server_iter = 1;

                foreach (string replica_url in this.server_addresses.Values)
                {
                    if (server_iter > n_replicas)
                    {
                        break;
                    }

                    if (!replica_url.Equals(this.server_url))
                    {
                        ServerInterface remote_server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), replica_url);
                        try
                        {
                            CreateAsyncDelegate RemoteDel = new CreateAsyncDelegate(remote_server.Create);
                            AsyncCallback RemoteCallback = new AsyncCallback(ServerCommunication.CreateAsyncCallBack);
                            IAsyncResult RemAr = RemoteDel.BeginInvoke(meeting_topic, min_attendees, slots, invitees, client_identifier, this.server_identifier, hops, logs_list, sent_version, RemoteCallback, null);
                        }
                        catch (System.Net.Sockets.SocketException se)
                        {
                            Console.WriteLine(se.Message);
                        }
                    }

                    server_iter++;
                }

                // TODO:  Por timer
                while (true)
                {
                    float current_messages = (float)this.receiving_create[meeting_topic].Count;

                    if (current_messages > (float)n_replicas / 2)
                    {
                        this.server_library.Create(meeting_topic, min_attendees, slots, invitees, client_identifier);
                        this.added_create.Add(meeting_topic);
                        this.CreateLog(meeting_topic, min_attendees, slots, invitees, client_identifier);
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
                }
            }
            this.pending_create.Remove(meeting_topic);
        }
        private void JoinBroadcast(string meeting_topic, List<string> slots, string client_identifier, int hops, Tuple<string, string> join_tuple, List<string> logs_list, int sent_version)
        {
            Console.WriteLine("join broadcast");    
            // adicionar || OU ADDED
            if (!this.pending_join.Contains(join_tuple) && this.server_library.GetVersion(meeting_topic) < sent_version)
            {
                this.pending_join.Add(join_tuple);

                int server_iter = 1;

                foreach (string replica_url in this.server_addresses.Values)
                {
                    if (server_iter > n_replicas)
                    {
                        break;
                    }

                    if (!replica_url.Equals(this.server_url))
                    {
                        ServerInterface remote_server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), replica_url);
                        try
                        {
                            JoinAsyncDelegate RemoteDel = new JoinAsyncDelegate(remote_server.Join);
                            AsyncCallback RemoteCallback = new AsyncCallback(ServerCommunication.JoinAsyncCallBack);
                            IAsyncResult RemAr = RemoteDel.BeginInvoke(meeting_topic, slots, client_identifier, this.server_identifier, hops, logs_list, sent_version, RemoteCallback, null);
                        }
                        catch (System.Net.Sockets.SocketException se)
                        {
                            Console.WriteLine(se.Message);
                        }
                    }

                    server_iter++;
                }

                // TODO:  Por timer
                while (true)
                {
                    float current_messages = (float)this.receiving_join[join_tuple].Count;

                    if (current_messages > (float)n_replicas / 2)
                    {
                        this.server_library.Join(meeting_topic, slots, client_identifier, sent_version);
                        this.added_join.Add(join_tuple);
                        this.JoinLog(meeting_topic, slots, client_identifier);
                        break;
                    }
                }
            }

            this.pending_join.Remove(join_tuple);
        }

        private void CloseBroadcast(string meeting_topic, string client_identifier, int hops, List<string> logs_list, int sent_version)
        {
            Console.WriteLine("close broadcast: " + this.server_library.GetVersion(meeting_topic) + " : " + sent_version);
            if (!this.pending_close.Contains(meeting_topic) && this.server_library.GetVersion(meeting_topic) < sent_version)
            {
                Console.WriteLine("if");
                this.pending_close.Add(meeting_topic);

                int server_iter = 1;

                foreach (string replica_url in this.server_addresses.Values)
                {
                    if (server_iter > n_replicas)
                    {
                        break;
                    }

                    if (!replica_url.Equals(this.server_url))
                    {
                        ServerInterface remote_server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), replica_url);
                        try
                        {
                            CloseAsyncDelegate RemoteDel = new CloseAsyncDelegate(remote_server.Close);
                            AsyncCallback RemoteCallback = new AsyncCallback(ServerCommunication.CloseAsyncCallBack);
                            IAsyncResult RemAr = RemoteDel.BeginInvoke(meeting_topic, client_identifier, this.server_identifier, hops, logs_list, sent_version, RemoteCallback, null);
                        }
                        catch (System.Net.Sockets.SocketException se)
                        {
                            Console.WriteLine(se.Message);
                        }
                    }

                    server_iter++;
                }

                // TODO:  Por timer
                while (true)
                {
                    float current_messages = (float)this.receiving_close[meeting_topic].Count;

                    if (current_messages > (float)n_replicas / 2)
                    {
                        this.server_library.Close(meeting_topic, client_identifier, sent_version);
                        this.added_close.Add(meeting_topic);
                        this.CloseLog(meeting_topic, client_identifier);
                        break;
                    }
                }
            }
            this.pending_close.Remove(meeting_topic);
        }

        private void CreateLog(string meeting_topic, int min_attendees, List<string> slots, List<string> invitees, string client_identifier)
        {           
            if(!logs_dictionary.ContainsKey(meeting_topic))
            {
                int write_version = server_library.GetVersion(meeting_topic);
                string json_log = new LogsParser().Create_ParseJSON(meeting_topic, write_version, min_attendees, slots, invitees, client_identifier);
                List<string> logs_list = new List<string>();
                logs_list.Add(json_log);
                this.logs_dictionary.TryAdd(meeting_topic, logs_list);
            }                
        }
        private void JoinLog(string meeting_topic, List<string> slots, string client_identifier)
        {
            int write_version = server_library.GetVersion(meeting_topic);
            string json_log = new LogsParser().Join_ParseJSON(meeting_topic, write_version, slots, client_identifier);
            List<string> logs_list = logs_dictionary[meeting_topic];
            logs_list.Add(json_log);
            this.logs_dictionary[meeting_topic] = logs_list;
        }

        private void CloseLog(string meeting_topic, string client_identifier)
        {
            int write_version = server_library.GetVersion(meeting_topic);
            string json_log = new LogsParser().Close_ParseJSON(meeting_topic, write_version, client_identifier);
            Console.WriteLine("dred");
            Console.WriteLine(json_log);
            List<string> logs_list = logs_dictionary[meeting_topic];
            logs_list.Add(json_log);
            this.logs_dictionary[meeting_topic] = logs_list;
        }

    }
}

