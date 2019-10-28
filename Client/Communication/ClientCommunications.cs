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

namespace MSDAD
{
    namespace Client.Comunication
    {        
        class ClientCommunications
        {
            int port;

            ClientLibrary clientLibrary;
            RemoteClient client;
            TcpChannel channel;
            ServerInterface server;
            
            List<MeetingView> meetingViews = new List<MeetingView>();

            public ClientCommunications(ClientLibrary clientLibrary, int port)
            {
                this.port = port;
                this.clientLibrary = clientLibrary;
            }
            public void Start()
            {
                channel = new TcpChannel(this.port);
                ChannelServices.RegisterChannel(channel, true);

                this.client = new RemoteClient(this);
                RemotingServices.Marshal(this.client, "RemoteClient", typeof(RemoteClient));
            }
  
            public void addMeetingView(string topic, List<string> rooms, int coord_port, int version, string state)
            {
                MeetingView meetingView;

                meetingView = new MeetingView(topic, rooms, coord_port, version, state);

                this.clientLibrary.AddMeetingView(meetingView);
            }    
            
            public void List(string status)
            {
                // TODO receive status from RemoteClient which in turn received it from the server

                // TODO passes the status to the ClientLibrary
            }

            public void Join()
            {
            }
        }
    }
    
}
