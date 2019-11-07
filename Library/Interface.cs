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

            void SendMeeting(string meeting_topic, int meeting_version, string meeting_state);
        }

        public interface ServerInterface
        {
            void Hello(string user, string ip, int port);
            void Ping(string message, string user);
            void Create(string meeting_topic, int min_attendees, List<string> slots, List<string> invitees, string client_identifier);
            void List(Dictionary<string, string> meeting_query, string client_identifier);
            void Join(string meeting_topic, List<string> slots, string client_identifier);
            void Close(string meeting_topic, string client_identifier);
            void Wait(int milliseconds);
        }

        public interface PCSInterface
        {
            void Send(string text);
        }

    }    
}
