using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Client.Library
{
    class List : Command
    {
        public override void Execute(ClientCommunication clientCommunication, int port_int)
        {
            List<MeetingView> meetingViews;
            
            clientCommunication.List(port_int);

            // TODO actualiza estado das meeting views
            meetingViews = clientCommunication.GetMeetingViews();

            foreach(MeetingView meetingView in meetingViews)
            {
                Console.WriteLine(meetingView.GetTopic());
            }
        }
    }
}
