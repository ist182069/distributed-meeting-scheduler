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
        class ClientMain
        {
            public const string PING_COMMAND = "ping";

            static void Main(string[] args)
            {
                ServerInterface server;
                string command, port;

                port = Console.ReadLine();
                TcpChannel channel = new TcpChannel(Int32.Parse(port));
                ChannelServices.RegisterChannel(channel, true);

                ClientUI.Display();
            }
        }
    }
    
}
