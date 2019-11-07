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
        public int server_port;
        public string server_ip, server_address, server_identifier;
        public ServerLibrary server_library;

        public Command(ref ServerLibrary server_library)
        {
            this.server_library = server_library;

            Init();
        }

        void Init()
        {
            this.server_identifier = this.server_library.ServerIdentifier;
            this.server_port = this.server_library.ServerPort;
            this.server_ip = this.server_library.ServerIP;
            this.server_address = ServerUtils.AssembleAddress(server_ip, server_port);
        }
        public abstract object Execute();
    }
}
