using MSDAD.Client.Exceptions;
using MSDAD.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Client.Commands

{
    class Initialize : Command
    {
        public Initialize(ref ClientLibrary client_library) : base(ref client_library)
        {
        }
        public override object Execute()
        {
            int n_replicas;
            try
            {
                n_replicas = this.remote_server.Hello(this.client_identifier, this.client_remoting, this.client_ip, this.client_port);
                base.client_library.NReplicas = n_replicas;                
            }
            catch (ServerCoreException e)
            {
                client_library.ClientCommunication.Destroy();
                throw e;
            }
            catch (System.Net.Sockets.SocketException se)
            {
                // TODO: ele aqui corre o algoritmo de forma diferente
                Console.WriteLine("Error! Server: \"" + base.client_library.ServerURL +"\" was not found...");
            }
            return null;
        }
    }
}
