using MSDAD.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;


namespace MSDAD
{ 
    namespace Client.Comunication {
        class RemoteClient : MarshalByRefObject, ClientInterface
        {
            ClientReceiveComm communication; 

            public RemoteClient(ClientReceiveComm communication)
            {
                this.communication = communication;
            }
            public void Ping(string message)
            {
                Console.WriteLine("Received message: " + message);
            }

            public void SendMeeting(string topic, List<string> rooms, int coord_port)
            {
                this.communication.Create(topic, rooms, coord_port);
            }
        }
    }
    
}
