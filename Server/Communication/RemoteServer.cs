using MSDAD.Library;
using System;
using System.Collections.Generic;

namespace MSDAD.Server.Communication
{
    class RemoteServer : MarshalByRefObject, ServerInterface
    {
        delegate void InvokeDelegate(string message);

        ServerCommunication server_communication;
        public RemoteServer(ServerCommunication server_communication)
        {
            this.server_communication = server_communication;
        }

        public void Create(string meeting_topic, int min_attendees, List<string> slots, List<string> invitees, string client_identifier, int hops)
        {
            this.server_communication.Create(meeting_topic, min_attendees, slots, invitees, client_identifier, hops);
        }

        public void List(Dictionary<string, string> meeting_query, string client_identifier)
        {
            this.server_communication.List(meeting_query, client_identifier);                
        }

        public void Join(string meeting_topic, List<string> slots, string client_identifier, int hops)
        {
            this.server_communication.Join(meeting_topic, slots, client_identifier, hops);
        }

        public void Close(string meeting_topic, string client_identifier, int hops)
        {
            this.server_communication.Close(meeting_topic, client_identifier, hops);
        }

        public void Ping(string message, string user)
        {
            Console.WriteLine("Received message: " + message);
            Console.WriteLine("Will broadcast it to all available clients... ");
            server_communication.BroadcastPing(message, user);
            Console.Write("Success!");
        }

        public void Wait(int milliseconds)
        {
            throw new NotImplementedException();
        }

        public void Hello(string client_identifier, string client_ip, int client_port)
        {
            server_communication.AddClientAddress(client_identifier, client_ip, client_port);
        }
    }
}
