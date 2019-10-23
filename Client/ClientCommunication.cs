using MSDAD.Library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
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
    namespace Client
    {        
        class ClientCommunication
        {
            int port;

            RemoteClient client;
            TcpChannel channel;
            ServerInterface server;

            List<MeetingView> meetingViews = new List<MeetingView>();

            public void Start(int port)
            {
                this.port = port;
                channel = new TcpChannel(port);
                ChannelServices.RegisterChannel(channel, true);

                this.client = new RemoteClient(this);
                RemotingServices.Marshal(this.client, "RemoteClient", typeof(RemoteClient));
            }
            public void GetRemoteServer()
            {                
                this.server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), "tcp://localhost:11000/RemoteServer");
            }
            public void Hello(int port)
            {
                this.server.Hello(port);
            }
            public void Ping()
            {
                string send_message;
                
                send_message = "ping";
                
                this.server.Ping(port, send_message);
            }
            public void Create(string topic, int minAttendees, List<string> rooms,List<int> invitees,int port)
            {
                this.server.Create(topic, minAttendees, rooms, invitees,port);
            }
            public void List(int port)
            {
                this.server.List(port);
            }
            public void Join(string topic, List<string> slots, int port)
            {
                try
                {
                    this.server.Join(topic, slots, port);
                } catch (ServerCommunicationException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            public void CreateMeeting(string topic, List<string> rooms, int coord_port)
            {
                MeetingView meetingView;

                meetingView = new MeetingView(topic, rooms, coord_port);

                this.AddView(meetingView);
            }

            public void AddView(MeetingView meetingView)
            {
                lock(this)
                {
                    meetingViews.Add(meetingView);
                }
            }
            
            public List<MeetingView> GetMeetingViews()
            {
                return this.meetingViews;
            }
        }
    }
    
}
