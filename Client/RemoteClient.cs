using MSDAD.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;


namespace MSDAD
{ 
    namespace Client {
        class RemoteClient : MarshalByRefObject, ClientInterface
        {
            Communication communication;

            public RemoteClient(Communication communication)
            {
                this.communication = communication;
            }
            public void Ping(string message)
            {
                Console.WriteLine(message);
            }
        }
    }
    
}
