using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD
{
    namespace Server
    {
        class ServerLibrary
        {
            ServerCommunication serverCommunication;            
            public ServerLibrary(ServerCommunication serverCommunication)
            {
                this.serverCommunication = serverCommunication;
            }
            
        }
    }    
}
