using MSDAD.Client.Exceptions;
using MSDAD.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Client.Commands
{
    class Close : Command
    {
        string[] words;

        public Close(ref ClientLibrary client_library, string[] words) : base(ref client_library)
        {
            this.words = words;
        }

        public override object Execute()
        {
            string topic;

            topic = this.words[1];

            try
            {
                this.remote_server.Close(topic, this.client_identifier, null, 0, null, Int32.MinValue);
            }
            catch (ServerCoreException sce)
            {
                Console.WriteLine(sce.Message);
            }
            catch (System.Net.Sockets.SocketException se)
            {

                this.remote_server = new ServerChange(ref base.client_library).Execute();
                if (this.remote_server!=null)
                {
                    int n_replicas = this.remote_server.Hello(this.client_identifier, this.client_remoting, this.client_ip, this.client_port);
                    base.client_library.NReplicas = n_replicas;
                    this.remote_server.Close(topic, this.client_identifier, null, 0, null, Int32.MinValue);
                }
                else
                {
                    throw new ClientLocalException("We cannot find anymore servers to connect to! Aborting...");
                }
            }

            return null;
        }

    }
}
