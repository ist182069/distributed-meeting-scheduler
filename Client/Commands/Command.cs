using MSDAD.Client.Comunication;
using MSDAD.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSDAD.Client.Commands
{
            
    abstract class Command
    {
        public int client_port;
        public string client_ip, client_address, client_identifier, client_remoting, server_url, command;
        public ClientLibrary client_library;
        public ServerInterface remote_server;

        public Command(ref ClientLibrary client_library)
        {
            this.client_library = client_library;

            Init();
        }

        public void Init()
        {
            this.client_identifier = this.client_library.ClientIdentifier;
            this.client_remoting = this.client_library.ClientRemoting;
            this.server_url = this.client_library.ServerURL;
            this.client_port = this.client_library.ClientPort;
            this.client_ip = this.client_library.ClientIP;
            this.client_address = ClientUtils.AssembleAddress(client_ip, client_port);
            Console.WriteLine("The URL to whom we are connectig to is: \"" + this.server_url + "\".");
            this.remote_server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), server_url);
        }
        public abstract object Execute();

        public void CrashClientProcess()
        {
            Console.Write("Crashing Client in...");
            Thread.Sleep(1000);
            Console.Write("3 ");
            Thread.Sleep(1000);
            Console.Write("2 ");
            Thread.Sleep(1000);
            Console.Write("1 ");
            Thread.Sleep(1000);
            Process.GetCurrentProcess().Kill();
        }
    }
}
