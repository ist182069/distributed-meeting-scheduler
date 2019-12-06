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
            while (true)
            {
                try
                {
                    n_replicas = this.remote_server.Hello(this.client_identifier, this.client_remoting, this.client_ip, this.client_port);
                    base.client_library.NReplicas = n_replicas;
                    break;
                }
                catch (ServerCoreException e)
                {
                    client_library.ClientCommunication.Destroy();
                    throw e;
                }
                catch (Exception exception) when (exception is System.Net.Sockets.SocketException || exception is System.IO.IOException)
                {
                    //ANTES: 
                    //throw new ServerCoreException("Error! Server: \"" + base.client_library.ServerURL + "\" was not found...");
                    //AGORA:
                    this.remote_server = new ServerChange(ref base.client_library).Execute(); //returns remote_server or null;

                    if (this.remote_server != null)
                    {
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("We cannot find anymore servers to connect to! Aborting...");
                        CrashClientProcess();
                    } //(vai á procura de outro server)
                }
            }
            
            return null;
        }
    }
}
