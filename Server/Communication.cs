using MSDAD.Library;
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
    namespace Server
    {
        class Communication
        {
            ArrayList portList = new ArrayList();
            RemoteServer remoteServer;
            TcpChannel channel;
            public void Start(string port)
            {                           
                channel = new TcpChannel(Int32.Parse(port));
                ChannelServices.RegisterChannel(channel, true);
                this.remoteServer = new RemoteServer(this);
                RemotingServices.Marshal(this.remoteServer, "RemoteServer", typeof(RemoteServer));
            }
            public void AddPortArray(int port)
            {
                lock(this)
                {
                    portList.Add(port);
                }
                
            }
            public void BroadcastPing(string message)
            {
                
                foreach (int port in this.portList)
                {
                    ClientInterface client = (ClientInterface)Activator.GetObject(typeof(ClientInterface), "tcp://localhost:" + port + "/RemoteClient");

                    client.Ping(message);
                }
             
            }

           
        }
    }
}
