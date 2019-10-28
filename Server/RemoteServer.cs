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

            public void Create(string topic, int minAttendees, List<string> rooms, List<int> invitees, int port)
            {
                this.communication.Create(topic, minAttendees, rooms, invitees, port);
            }

            public void List(int port)
            {
                // TODO de momento nao retorna nada. Eventualmente devolve os estados do sistema                
            }

            public void Join(string topic, List<string> slots, int port)
            {
                this.communication.Join(topic, slots, port);
            }

            public void Close(string meeting_topic, int port)
            {
                this.communication.Close(meeting_topic, port);
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
