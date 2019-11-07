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
            ClientCommunication communications; 

            public RemoteClient(ClientCommunication communications)
            {
                this.communications = communications;
            }
            public void Ping(string message)
            {
                Console.WriteLine("Received message: " + message);
            }

            public void SendMeeting(string topic, int version, string state)
            {
                this.communications.AddMeetingView(topic, version, state);
            }
        }
    }
    
}
