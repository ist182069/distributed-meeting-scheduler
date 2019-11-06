using MSDAD.PCS.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
        private const string ADD_ROOM = "AddRoom";

        private const string PCS = "PCS";
        private const string SERVER = "Server";

        TcpChannel channel;

        RemotePCS remotePCS;
        PCSLibrary pcsLibrary;

        public PCSParser()
        {
            this.pcsLibrary = new PCSLibrary();
        }
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
            string[] lines, words;

            // assuming it reads an entire script
            lines = script.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                words = line.Split(' ');
                this.ParseLine(words);
            }

        }

        private void Launch()
        {
            Process process = new Process();
            process.StartInfo.FileName = "demo.exe";
            process.StartInfo.Arguments = "teste";
            process.Start();
            process.WaitForExit();                
        }

        private void ParseLine(string[] words)
        {
            this.pcsLibrary.SetWords(words);

            switch(words[0])
            {
                case ADD_ROOM:
                    new AddRoom(ref this.pcsLibrary).Execute();
                    break;
                case SERVER:
                    new Server(ref this.pcsLibrary).Execute();
                    break;
                default:
                    Console.WriteLine("Error! The command you have inserted is not a valid one...");
                    break;

            }
        }
    }
}
