using MSDAD.Library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;
using MSDAD.Client.Exceptions;
using System.Runtime.Remoting.Messaging;

namespace MSDAD.Client.Comunication
{        
    class ClientCommunication
    {
        int client_port;
        string client_ip, client_identifier, client_remoting;

        ClientLibrary client_library;
        RemoteClient remote_client;
        TcpChannel channel;

        List<Tuple<string, int>> gossip_tuples = new List<Tuple<string, int>>();

        public delegate void SendMeetingToClientGossipAsyncDelegate(string meeting_topic, int meeting_version, string meeting_state, string extraInfo, List<string> client_list);

        public static void SendMeetingToClientGossipAsyncCallBack(IAsyncResult ar)
        {
            SendMeetingToClientGossipAsyncDelegate del = (SendMeetingToClientGossipAsyncDelegate)((AsyncResult)ar).AsyncDelegate;
            return;
        }

        public ClientCommunication(ClientLibrary client_library)
        {
            this.client_library = client_library;
            this.client_identifier = client_library.ClientIdentifier;
            this.client_port = client_library.ClientPort;
            this.client_ip = client_library.ClientIP;
            this.client_remoting = client_library.ClientRemoting;
        }
        public void Start()
        {
            try
            {
                channel = new TcpChannel(this.client_port);
                ChannelServices.RegisterChannel(channel, true);
            }
            catch (SocketException e)
            {
                throw new ClientLocalException(ErrorCodes.ALREADY_USED_PORT);
            }

            this.remote_client = new RemoteClient(this);
            RemotingServices.Marshal(this.remote_client, client_remoting, typeof(RemoteClient));
        }

        public void Destroy()
        {
            ChannelServices.UnregisterChannel(this.channel);
        }
  
        public void AddMeetingView(string topic, int version, string state, string extraInfo)
        {
            MeetingView meetingView;

            meetingView = new MeetingView(topic, version, state, extraInfo);

            this.client_library.AddMeetingView(meetingView);
        }

        public void AddMeetingViewGossip(string topic, int version, string state, string extraInfo, List<string> client_list)
        {
            Tuple<string, int> gossip_tuple = new Tuple<string, int>(topic, version);

            if(!this.gossip_tuples.Contains(gossip_tuple))
            {
                this.gossip_tuples.Add(gossip_tuple);

                MeetingView meetingView;

                meetingView = new MeetingView(topic, version, state, extraInfo);

                this.client_library.AddMeetingView(meetingView);

                this.SendLogNMessages(topic, client_list);
            }            
        }

        public void Status()
        {
            int port;
            string client_identifier, client_ip, client_url, client_remoting, server_url;
            List<MeetingView> meetingViews;

            client_identifier = this.client_library.ClientIdentifier;
            client_ip = this.client_library.ClientIP;
            port = this.client_library.ClientPort;
            client_remoting = this.client_library.ClientRemoting;
            client_url = ClientUtils.AssembleRemotingURL(client_ip, port, client_remoting);
            server_url = this.client_library.ServerURL;
            meetingViews = this.client_library.GetMeetingViews();

            Console.WriteLine("client id: " + client_identifier);
            Console.WriteLine("remoting url: " + client_url);
            Console.WriteLine("server url: " + server_url);

            foreach (MeetingView meetingView in meetingViews)
            {
                Console.WriteLine("Topic: " + meetingView.MeetingTopic);
                Console.WriteLine("State: " + meetingView.MeetingState);
                Console.WriteLine("Version: " + meetingView.MeetingVersion);
                Console.WriteLine("Info: " + meetingView.MeetingInfo);
            }
        }

        public void SendLogNMessages(string meeting_topic, List<string> client_addresses)
        {
            Console.WriteLine("bora propagar!!!");
            int number_clients = client_addresses.Count;

            if (number_clients != 0)
            {
                double clients_double = Convert.ToDouble(number_clients);
                double clients_log = Math.Log(clients_double, 2);

                // se for 0 e porque so havia um
                if (clients_log != 0)
                {
                    double log_round = Math.Ceiling(clients_log);
                    string[] random_clients = this.PickNRandomClients((int)log_round, client_addresses);

                    for (int i = 0; i < random_clients.Length; i++)
                    {
                        ClientInterface client = (ClientInterface)Activator.GetObject(typeof(ClientInterface), "tcp://" + random_clients[i]);

                        SendMeetingToClientGossipAsyncDelegate RemoteDel = new SendMeetingToClientGossipAsyncDelegate(client.SendMeetingGossip);
                        AsyncCallback RemoteCallback = new AsyncCallback(ClientCommunication.SendMeetingToClientGossipAsyncCallBack);
                        IAsyncResult RemAr = RemoteDel.BeginInvoke(meeting_topic, 1, "OPEN", null, client_addresses, RemoteCallback, null);
                    }
                }

            }
            Console.WriteLine("propagou!!!");
        }

        private string[] PickNRandomClients(int n_clients, List<string> client_addresses)
        {
            int insertion_counter = 0, random_int, break_condition;
            string random_address, random_remoting_id;
            string[] selected_clients;
            Random random;
            
            selected_clients = new string[n_clients];
            random = new Random();

            Console.WriteLine("pick and send");
            Console.WriteLine("number of clients:" + n_clients);
            while (true)
            {

                random_int = random.Next(0, (n_clients+1));
                random_address = client_addresses[random_int];
                random_remoting_id = CommonUtils.GetRemotingIdFromUrl("tcp://" + random_address);

                Console.WriteLine("chosen address:" + random_address);
                if (!selected_clients.Contains(random_address) && !this.client_remoting.Equals(random_remoting_id))
                {
                    Console.WriteLine("Adicionou");
                    selected_clients[insertion_counter] = random_address;
                    insertion_counter++;
                }
                if (insertion_counter == n_clients)
                {
                    break;
                }
            }

            return selected_clients;
        }
    }
}
