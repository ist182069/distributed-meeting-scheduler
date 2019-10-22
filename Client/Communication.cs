using MSDAD.Library;
using System;
using System.Collections.Generic;
using System.Linq;
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
            TcpChannel channel;
            ServerInterface server;
            public void Start(string port)
            {
                channel = new TcpChannel(Int32.Parse(port));
                ChannelServices.RegisterChannel(channel, true);
            }
            public void GetRemoteServer()
            {                
                this.server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), "tcp://localhost:11000/RemoteServer");
            }
            public string Ping()
            {
                string recv_message, send_message;

                send_message = "ping";

                recv_message = this.server.Ping(send_message);

                return recv_message; 
            }
        }
    }
    
}
