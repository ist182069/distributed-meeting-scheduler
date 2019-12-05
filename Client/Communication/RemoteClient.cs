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

            public void SendMeeting(string meeting_topic, int meeting_version, string meeting_state, string extraInfo)
            {
                this.client_communication.AddMeetingView(meeting_topic, meeting_version, meeting_state, extraInfo);
            }

            public void SendMeetingGossip(string meeting_topic, int meeting_version, string meeting_state, string extraInfo, List<string> client_list)
            {               
                this.client_communication.AddMeetingViewGossip(meeting_topic, meeting_version, meeting_state, extraInfo, client_list);
            }

            public void Status()
            {
                this.client_communication.Status();
            }
        }
    }
    
}
