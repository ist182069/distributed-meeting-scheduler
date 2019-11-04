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

            void SendMeeting(string topic, int version, string state);
        }

        public interface ServerInterface
        {
            void Hello(string ip, int port);
            void Ping(string ip, int port, string message);
            void List(Dictionary<string, int> meetingQuery, string ip, int port);
            void Create(string topic, int minAttendees, List<string> rooms, List<string> invitees, string ip, int port);
            void Join(string topic, List<string> slots, string ip, int port);
            void Close(string topic, string ip, int port);
            void Wait(int milliseconds);
        }
    }    
}
