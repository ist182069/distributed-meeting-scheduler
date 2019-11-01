using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD
{
    namespace Client
    {
        public class MeetingView
        {
            private string state, topic;
            private int version;
            
            
            public MeetingView(string topic, int version, string state)
            {
                this.topic = topic;
                this.version = version;
                this.state = state;
            }

            public string GetTopic()
            {
                return this.topic;
            }          

            public int GetVersion()
            {
                return this.version;
            }

            public string GetState()
            {
                return this.state;
            }
        }
    }
    
}
