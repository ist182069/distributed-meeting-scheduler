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
            ClientCommunications communications; 

            public RemoteClient(ClientCommunications communications)
            {
                this.communications = communications;
            }
            public void Ping(string message)
            {
                Console.WriteLine("Received message: " + message);
            }

            public void SendMeeting(string topic, List<string> rooms, string coordinator, int version, string state)
            {
                this.communications.AddMeetingView(topic, rooms, coordinator, version, state);
            }
        }
    }
    
}
