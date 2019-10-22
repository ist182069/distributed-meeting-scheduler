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
        }

        public interface ServerInterface
        {
            void Hello(int port);
            void Ping(int port, string message);
            string List();
            void Create(String meeting_topic);
            void Join(String meeting_topic);
            void Close(String meeting_topic);
            void Wait(int milliseconds);
        }
    }    
}
