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
                this.server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), "tcp://localhost:11000/ServerInterface");
            }
            public string Ping()
            {
                string message;

                message = this.server.Ping();

                return message; 
            }
        }
    }
    
}
