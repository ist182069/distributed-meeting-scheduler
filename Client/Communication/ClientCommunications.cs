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

namespace MSDAD.Client.Comunication
{        
    class ClientCommunication
    {
        int port;
        string ip, client_identifier;

        ClientLibrary clientLibrary;
        RemoteClient client;
        TcpChannel channel;
            
        List<MeetingView> meetingViews = new List<MeetingView>();

        public ClientCommunication(ClientLibrary clientLibrary, string client_identifier, string ip, int port)
        {
            this.client_identifier = client_identifier;
            this.port = port;
            this.ip = ip;
            this.clientLibrary = clientLibrary;
        }
        public void Start()
        {
            channel = new TcpChannel(this.port);
            ChannelServices.RegisterChannel(channel, true);

            this.client = new RemoteClient(this);
            RemotingServices.Marshal(this.client, client_identifier, typeof(RemoteClient));
        }
  
        public void AddMeetingView(string topic, int version, string state)
        {
            MeetingView meetingView;

            meetingView = new MeetingView(topic, version, state);

            this.clientLibrary.AddMeetingView(meetingView);
        }    

        public void Join()
        {
        }
    }
}
