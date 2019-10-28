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
            private int coord_port;
            private string topic;
            private int version;
            private string state;
            private List<string> rooms;
            
            public MeetingView(string topic, List<string> rooms, int coord_port,int version,string state)
            {
                this.coord_port = coord_port;
                this.topic = topic;
                this.rooms = rooms;
                this.version = version;
                this.state = state;
            }

            public int GetPort()
            {
                return this.coord_port;
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
