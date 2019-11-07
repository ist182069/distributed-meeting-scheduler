using MSDAD.Library;
using System;
using System.Collections.Generic;

namespace MSDAD.Server.Communication
{
    class RemoteServer : MarshalByRefObject, ServerInterface
    {
        delegate void InvokeDelegate(string message);

        ServerCommunication communication;
        public RemoteServer(ServerCommunication communication)
        {
            this.communication = communication;
        }

        public void Create(string topic, int minAttendees, List<string> rooms, List<string> clients, string user)
        {
            this.communication.Create(topic, minAttendees, rooms, clients, user);
        }

        public void List(Dictionary<string, string> meetingQuery, string user)
        {
            this.communication.List(meetingQuery, user);                
        }

        public void Join(string topic, List<string> slots, string user)
        {
            this.communication.Join(topic, slots, user);
        }

        public void Close(string meeting_topic, string user)
        {
            this.communication.Close(meeting_topic, user);
        }

        public void Ping(string message, string user)
        {
            Console.WriteLine("Received message: " + message);
            Console.WriteLine("Will broadcast it to all available clients... ");
            communication.BroadcastPing(message, user);
            Console.Write("Success!");
        }

        public void Wait(int milliseconds)
        {
            throw new NotImplementedException();
        }

        public void Hello(string user, string ip, int port)
        {
            communication.AddClientAddress(user, ip, port);
        }
    }
}
