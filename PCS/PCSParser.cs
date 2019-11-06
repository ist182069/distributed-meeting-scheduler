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

namespace MSDAD.PCS
{
    class PCSParser
    {
        private const string PCS = "PCS";
        
        TcpChannel channel;

        RemotePCS remotePCS;

        public void Start(int port)
        {
            Console.Write("Starting up PCS remoting module... ");
            this.channel = new TcpChannel(port);
            ChannelServices.RegisterChannel(channel, true);
            this.remotePCS = new RemotePCS(this);
            RemotingServices.Marshal(this.remotePCS, PCS, typeof(RemotePCS));
            Console.WriteLine("Success!");
        }

        public void WaitForCommands()
        {
            Console.WriteLine("Waiting for Puppet Master commands...");
            while (true) ;
        }

        public void Execute(string script)
        {            
            string[] lines;

            Console.WriteLine("chegou");

            // assuming it reads an entire script

            lines = script.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                Console.WriteLine(line);
            }

        }
    }
}
