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

            Console.WriteLine(client_number + " : " + n_replicas);
            try_replica = (client_number % n_replicas) + 1;
            Console.WriteLine("try_replica = (client_number % n_replicas) + 1; " + try_replica);

            for(int i = 0; i < n_replicas; i++)
            {
                server_url = "tcp://localhost:" + (try_replica + 3000) + "/Server" + try_replica;
                Console.WriteLine(server_url);
                try
                {
                    this.clientLibrary.ServerURL = server_url;
                    this.remote_server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), server_url);
                    this.remote_server.IsAlive();
                    break;
                    
                    // cena fodix que pensei em que passas o comando original aqui para dentro
                }
                catch (System.Net.Sockets.SocketException se)
                {
                    try_replica = ((try_replica + 1) % n_replicas) + 1;
                    Console.WriteLine(try_replica);
                }                
            }

            return this.remote_server;

        }      
    }
}
