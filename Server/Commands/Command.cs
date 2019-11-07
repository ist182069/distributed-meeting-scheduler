using MSDAD.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Server.Commands
{
            
    abstract class Command
    {
        public int port;
        public string ip, client_address, server_identifier, server_url;
        public ServerLibrary serverLibrary;
        public ServerInterface server;

        public Command(ref ServerLibrary serverLibrary)
        {
            this.serverLibrary = serverLibrary;

            Init();
        }

        void Init()
        {
            this.server_identifier = this.serverLibrary.GetServerIdentifier();
            this.port = this.serverLibrary.GetServerPort();
            this.ip = this.serverLibrary.GetServerIP();
            this.client_address = ServerUtils.AssembleAddress(ip, port);
        }
        public abstract object Execute();
    }
}
