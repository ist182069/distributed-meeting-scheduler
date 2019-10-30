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
            private string coordinator, topic;
            private int version;
            private string state;
            private List<string> rooms;
            
            public MeetingView(string topic, List<string> rooms, string coordinator, int version, string state)
            {
                this.coordinator = coordinator;
                this.topic = topic;
                this.rooms = rooms;
                this.version = version;
                this.state = state;
            }

            public string GetAddress()
            {
                return this.coordinator;
            }

            public string GetTopic()
            {
                return this.topic;
            }

            public List<string> GetRooms()
            {
                return this.rooms;
            }

            public int getVersion()
            {
                return this.version;
            }

            public string getState()
            {
                return this.state;
            }
        }
    }
    
}
