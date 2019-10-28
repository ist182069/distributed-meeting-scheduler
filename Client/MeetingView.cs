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
            int version;

            List<string> rooms;
            
            public MeetingView(string topic, List<string> rooms, int coord_port,int version)
            {
                this.coord_port = coord_port;
                this.topic = topic;
                this.rooms = rooms;
                this.version = version;
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
        }
    }
    
}
