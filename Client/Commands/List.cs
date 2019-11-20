using MSDAD.Client.Comunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSDAD.Client.Commands

{
    class List : Command
    {
        public List(ref ClientLibrary client_library) : base(ref client_library)
        {

        }
        public override object Execute()
        {
            List<MeetingView> meeting_views = this.client_library.GetMeetingViews();
            Dictionary<string,string> meeting_query = new Dictionary<string,string>();

            foreach (MeetingView mV in meeting_views)
            {
                Console.WriteLine(mV.MeetingTopic + " " + mV.MeetingState + " " + mV.MeetingInfo + " " + mV.MeetingVersion + " "); 
                meeting_query.Add(mV.MeetingTopic, mV.MeetingState);
            }

            this.remote_server.List(meeting_query, this.client_identifier);

            foreach (MeetingView mV in meeting_views)
            {
                Console.WriteLine("Topic:"+mV.MeetingTopic + " State:" + mV.MeetingState + "\nVersion:" + mV.MeetingVersion);
                if(mV.MeetingInfo != null)
                {
                    Console.WriteLine(mV.MeetingInfo);
                }
                Console.Write("\n");
            }

            return null;
        }
    }
}
