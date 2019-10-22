using MSDAD.Library;
using System;

namespace MSDAD
{
    namespace Server
    {
        class RemoteServer : MarshalByRefObject, ServerInterface
        {
            delegate void InvokeDelegate(string message);

            Communication communication;
            public RemoteServer(Communication communication)
            {
                this.communication = communication;
            }
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

            public string Ping(string recv_message)
            {
                string send_message;

                Console.WriteLine(recv_message);

                send_message = "pong";

                return send_message;
            }

            public void Wait(int milliseconds)
            {
                throw new NotImplementedException();
            }
        }
    }
    
}
