using MSDAD.Library;
using System;
using System.Collections.Generic;

namespace MSDAD
{
    namespace Server
    {
        class RemoteServer : MarshalByRefObject, ServerInterface
        {
            delegate void InvokeDelegate(string message);

            ServerCommunication communication;
            public RemoteServer(ServerCommunication communication)
            {
                this.communication = communication;
            }
            public void Close(string meeting_topic)
            {
                throw new NotImplementedException();
            }

            public void Create(string topic, int minAttendees, List<string> rooms, List<int> invitees, int port)
            {
                this.communication.Create(topic, minAttendees, rooms, invitees, port);
            }

            public void Join(string meeting_topic)
            {
                throw new NotImplementedException();
            }

            public string List()
            {
                return communication.List();
            }

            public void Ping(int port, string message)
            {
                Console.WriteLine("Received message: " + message);
                Console.WriteLine("Will broadcast it to all available clients... ");
                communication.BroadcastPing(port, message);
                Console.Write("Success!");
            }

            public void Wait(int milliseconds)
            {
                throw new NotImplementedException();
            }

            public void Hello(int port)
            {
                communication.AddPortArray(port);
            }
        }
    }
    
}
