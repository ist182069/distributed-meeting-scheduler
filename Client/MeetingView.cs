using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD
{
    namespace Client
    {
        class MeetingView
        {
            int coord_port;
            string topic;

            List<string> rooms;
            
            public MeetingView(string topic, List<string> rooms, int coord_port)
            {
                this.coord_port = coord_port;
                this.topic = topic;
                this.rooms = rooms;
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
        }
    }
    
}
