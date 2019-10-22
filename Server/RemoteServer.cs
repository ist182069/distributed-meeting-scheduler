using MSDAD.Library;
using System;

namespace MSDAD
{
    namespace Server
    {
        class RemoteServer : MarshalByRefObject, ServerInterface
        {
            public void Close(string meeting_topic)
            {
                throw new NotImplementedException();
            }

            public void Create(string meeting_topic)
            {
                throw new NotImplementedException();
            }

            public void Join(string meeting_topic)
            {
                throw new NotImplementedException();
            }

            public string List()
            {
                throw new NotImplementedException();
            }

            public string Ping()
            {
                string message = "Ping";
                return message;
            }

            public void Wait(int milliseconds)
            {
                throw new NotImplementedException();
            }
        }
    }
    
}
