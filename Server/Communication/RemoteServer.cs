using MSDAD.Library;
using System;
using System.Collections.Generic;
using System.Threading;

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

        public void Create(string meeting_topic, int min_attendees, List<string> slots, List<string> invitees, string client_identifier, string create_replica_identifier, int hops, List<string> logs_list, int version)
        {
            Thread.Sleep(this.server_communication.Delay());
            this.server_communication.Create(meeting_topic, min_attendees, slots, invitees, client_identifier, create_replica_identifier, hops, logs_list, version);
        }

        public void List(Dictionary<string, string> meeting_query, string client_identifier)
        {
            Thread.Sleep(this.server_communication.Delay());
            this.server_communication.List(meeting_query, client_identifier);                
        }

        public void Join(string meeting_topic, List<string> slots, string client_identifier, string join_replica_identifier, int hops, List<string> logs_list, int version)
        {
            Thread.Sleep(this.server_communication.Delay());
            this.server_communication.Join(meeting_topic, slots, client_identifier, join_replica_identifier, hops, logs_list, version);
        }
        public void Close(string meeting_topic, string client_identifier, string server_identifier, int hops, List<string> logs_list, int version)
        {
            Thread.Sleep(this.server_communication.Delay());
            this.server_communication.Close(meeting_topic, client_identifier, server_identifier, hops, logs_list, version);
        }
        public void Ping(string message, string user)
        {
            Thread.Sleep(this.server_communication.Delay());
            Console.WriteLine("Received message: " + message);
            Console.WriteLine("Will broadcast it to all available clients... ");
            server_communication.BroadcastPing(message, user);
            Console.Write("Success!");
        }

        public void Wait(int milliseconds)
        {
            throw new NotImplementedException();
        }

        public void Hello(string client_identifier, string client_remoting, string client_ip, int client_port)
        {
            Thread.Sleep(this.server_communication.Delay());
            server_communication.AddClientAddress(client_identifier, client_remoting, client_ip, client_port);
        }

        public void Status()
        {
            Thread.Sleep(this.server_communication.Delay());
            server_communication.Status();
        }

        public void NReplicasUpdate(int n_replicas)
        {
            server_communication.setNReplica(n_replicas);
        }

        public void IsAlive()
        {
            //returns exception if not alive.
        }

        public void GetMeeting(string meeting_topic, string server_identifier)
        {
            Thread.Sleep(this.server_communication.Delay());
            server_communication.GetMeeting(meeting_topic, server_identifier);
        }

        public void SendMeeting(string meeting_topic, int version, List<string> logs_list, string server_identifier)
        {
            Thread.Sleep(this.server_communication.Delay());
            server_communication.SendMeeting(meeting_topic, version, logs_list, server_identifier);
        }        
    }
}
