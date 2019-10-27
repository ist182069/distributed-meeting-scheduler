using MSDAD.Client.Comunication;
using MSDAD.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD
{
    namespace Client
    {
        namespace Commands {
            
            abstract class Command
            {
                public int port;
                public ClientLibrary clientLibrary;
                public ServerInterface server;

                public Command(ref ClientLibrary clientLibrary)
                {
                    this.clientLibrary = clientLibrary;

                    Init();
                }

                void Init()
                {
                    this.port = this.clientLibrary.GetPort();
                    this.server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), "tcp://localhost:11000/RemoteServer");
                    this.server.Hello(this.port);
                }
                public abstract object Execute();
            }
        }        
    }    
}
