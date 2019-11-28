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
        private const string CRASH = "Crash";
        private const string WAIT = "Wait";
        private const string CLIENT = "Client";
        private const string PCS = "PCS";
        private const string SERVER = "Server";
        private const string FREEZE = "Freeze";
        private const string UNFREEZE = "Unfreeze";
        private const string STATUS = "Status";

        private bool write = true;

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
            PCSUtils.DeleteLocations();
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
                case CLIENT:
                    new Client(ref this.pcsLibrary).Execute();
                    break;
                case SERVER:
                    new Server(ref this.pcsLibrary, write).Execute();
                    write = false;
                    break;
                case CRASH:
                    new Crash(ref this.pcsLibrary).Execute();
                    break;
                case FREEZE:
                    new Freeze(ref this.pcsLibrary).Execute();
                    break;
                case UNFREEZE:
                    new Unfreeze(ref this.pcsLibrary).Execute();
                    break;
                case STATUS:
                    new Status(ref this.pcsLibrary).Execute();
                    break;
                case WAIT:
                    System.Threading.Thread.Sleep(Int32.Parse(words[1]));
                    break;
                default:
                    Console.WriteLine("Error! The command you have inserted is not a valid one...");
                    break;

            }
        }
    }
}
