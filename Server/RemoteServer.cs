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

            public void Create(string topic, int minAttendees, List<string> rooms, List<string> clients, string ip, int port)
            {
                this.communication.Create(topic, minAttendees, rooms, clients, ip, port);
            }

            public void List(Dictionary<string, int> meetingQuery, string ip, int port)
            {
                this.communication.List(meetingQuery, ip, port);                
            }

            public void Join(string topic, List<string> slots, string ip, int port)
            {
                this.communication.Join(topic, slots, ip,  port);
            }

            public void Close(string meeting_topic, string ip, int port)
            {
                this.communication.Close(meeting_topic, ip, port);
            }

            public void Ping(string ip, int port, string message)
            {
                Console.WriteLine("Received message: " + message);
                Console.WriteLine("Will broadcast it to all available clients... ");
                communication.BroadcastPing(ip, port, message);
                Console.Write("Success!");
            }

            public void Wait(int milliseconds)
            {
                throw new NotImplementedException();
            }

            public void Hello(string ip, int port)
            {
                communication.AddClientAddress(ip, port);
            }
        }
    }
    
}
