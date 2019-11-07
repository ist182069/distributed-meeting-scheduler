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
            ClientCommunication client_communication; 

            public RemoteClient(ClientCommunication client_communication)
            {
                this.client_communication = client_communication;
            }
            public void Ping(string message)
            {
                Console.WriteLine("Received message: " + message);
            }

            public void SendMeeting(string meeting_topic, int meeting_version, string meeting_state)
            {
                this.client_communication.AddMeetingView(meeting_topic, meeting_version, meeting_state);
            }
        }
    }
    
}
