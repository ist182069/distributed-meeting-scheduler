using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD
{
    namespace Client
    {
        namespace Library {
            
            abstract class Command
            {
                public abstract void Execute(ClientCommunication clientCommunication, int port_int);
            }
        }        
    }    
}
