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

namespace MSDAD.Client.Comunication
{        
    class ClientCommunication
    {
        int client_port;
        string client_ip, client_identifier, client_remoting;

        ClientLibrary client_library;
        RemoteClient remote_client;
        TcpChannel channel;

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
    }
}
