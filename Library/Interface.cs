using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD
{
    namespace Library
    {
        public interface ClientInterface
        {
            void Ping(string message);

            void SendMeeting(string topic, List<string> rooms, int coord_port);
        }

        public interface ServerInterface
        {
            void Hello(int port);
            void Ping(int port, string message);
            void List(int port);
            void Create(string topic, int minAttendees, List<string> rooms, List<int> invitees, int port);
            void Join(string topic, List<string> slots, int port);
            void Close(string meeting_topic);
            void Wait(int milliseconds);
        }
    }    
}
