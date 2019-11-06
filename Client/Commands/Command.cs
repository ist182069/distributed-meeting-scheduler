using MSDAD.Client.Comunication;
using MSDAD.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Client.Commands
{
            
    abstract class Command
    {
        public int port;
        public string ip, client_address, user;
        public ClientLibrary clientLibrary;
        public ServerInterface server;

        public Command(ref ClientLibrary clientLibrary)
        {
            this.clientLibrary = clientLibrary;

            Init();
        }

        void Init()
        {
            this.user = this.clientLibrary.GetUser();
            this.port = this.clientLibrary.GetPort();
            this.ip = this.clientLibrary.GetIP();
            this.client_address = ClientUtils.AssembleClientAddress(ip, port);
            this.server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), "tcp://localhost:11000/RemoteServer");
            this.server.Hello(this.user, this.ip, this.port);
        }
        public abstract object Execute();
    }
}
