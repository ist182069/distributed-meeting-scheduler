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
        class Communication
        {
            RemoteClient client;
            TcpChannel channel;
            ServerInterface server;
            int port;
            
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
        }
    }
    
}
