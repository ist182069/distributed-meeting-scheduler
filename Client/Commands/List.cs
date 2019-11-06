using MSDAD.Client.Comunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Client.Commands
{
    class List : Command
    {
        public List(ref ClientLibrary clientLibrary) : base(ref clientLibrary)
        {

        }
        public override object Execute()
        {
            List<MeetingView> meetingViews = this.clientLibrary.GetMeetingViews();
            Dictionary<string,string> meetingQuery = new Dictionary<string,string>();

            foreach (MeetingView mV in meetingViews)
            {
                meetingQuery.Add(mV.GetTopic(), mV.GetState());
            }

            this.server.List(meetingQuery, ip, port);

            foreach (MeetingView mV in meetingViews)
            {
                Console.WriteLine("Topic:"+mV.GetTopic() + " State:" + mV.GetState() + "\nVersion:" + mV.GetVersion() + "\n");
            }

            return null;
        }
    }
}
