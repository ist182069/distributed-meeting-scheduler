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
    namespace Client.Comunication
    {
        class ClientSendComm
        {
            int port;
            ServerInterface server;
            public ClientSendComm(int port)
            {
                this.port = port;
            }
            public void Start()
            {
                this.server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), "tcp://localhost:11000/RemoteServer");
            }
            public void Hello()
            {
                this.server.Hello(this.port);
            }
            public void Ping()
            {
                string send_message;

                send_message = "ping";

                this.server.Ping(this.port, send_message);
            }
            public void Create(string topic, int minAttendees, List<string> rooms, List<int> invitees, int port)
            {
                this.server.Create(topic, minAttendees, rooms, invitees, port);
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
                }
                catch (ServerCommunicationException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }   
}
