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
            void Hello(string user, string ip, int port);
            void Ping(string message, string user);
            void List(Dictionary<string, string> meetingQuery, string user);
            void Create(string topic, int minAttendees, List<string> rooms, List<string> invitees, string user);
            void Join(string topic, List<string> slots, string user);
            void Close(string topic, string user);
            void Wait(int milliseconds);
        }

        public interface PCSInterface
        {
            void Send(string text);
        }

    }    
}
