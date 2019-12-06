using MSDAD.Client;
using MSDAD.Client.Commands;
using MSDAD.Client.Exceptions;
using MSDAD.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD
{
    class ServerChange
    {
        ClientLibrary clientLibrary;
        Dictionary<string, string> server_addresses = new Dictionary<string, string>();
        ServerInterface remote_server = null;

        public ServerChange(ref ClientLibrary clientLibrary)
        {
            this.clientLibrary = clientLibrary;
        }

        public ServerInterface Execute()
        {
            int client_number, n_replicas, try_replica;
            string server_url;

            client_number = Int32.Parse(this.clientLibrary.ClientIdentifier.Remove(0,1));
            n_replicas = this.clientLibrary.NReplicas;

            Console.WriteLine("Server knows \"" + client_number + "\" clients...");
            Console.WriteLine("Server has \"" + n_replicas + "\" replicas...");
            try_replica = (client_number % n_replicas) + 1;
            Console.WriteLine("Since current replica has failed will try to connect to replica \"" + try_replica + "\"...");

            for(int i = 0; i < n_replicas; i++)
            {
                server_url = "tcp://localhost:" + (try_replica + 3000) + "/Server" + try_replica;
                Console.WriteLine("Replica url: \"" + server_url + "\".");
                try
                {
                    this.clientLibrary.ServerURL = server_url;
                    this.remote_server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), server_url);
                    this.remote_server.IsAlive();
                    break;
                }
                catch (Exception exception) when (exception is System.Net.Sockets.SocketException || exception is System.IO.IOException)
                {
                    try_replica = ((try_replica + 1) % n_replicas) + 1;
                    Console.WriteLine(try_replica);
                }                
            }

            return this.remote_server;

        }      
    }
}
