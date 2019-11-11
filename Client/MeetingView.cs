using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Client
{
    public class MeetingView
    {
        private string meeting_state, meeting_topic, extra_info;
        private int meeting_version;
            
            
        public MeetingView(string meeting_topic, int meeting_version, string meeting_state, string extra_info)
        {
            this.meeting_topic = meeting_topic;
            this.meeting_version = meeting_version;
            this.meeting_state = meeting_state;
            this.extra_info = extra_info;
        }

        public string MeetingTopic
        {
            get
            {
                return this.meeting_topic;
            }
        }          

        public int MeetingVersion
        {
            get
            {
                return this.meeting_version;
            }
        }

        public string MeetingState
        {
            get
            {
                return this.meeting_state;
            }
        }

        public string MeetingInfo
        {
            get
            {
                return this.extra_info;
            }
        }
    }
}