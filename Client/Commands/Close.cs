using MSDAD.Client.Exceptions;
using MSDAD.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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

            while (true)
            {
                try
                {
                    this.remote_server.Close(topic, this.client_identifier, null, 0, null, Int32.MinValue);
                    break;
                }
                catch (ServerCoreException sce)
                {
                    Console.WriteLine(sce.Message);
                }
                catch (Exception exception) when (exception is System.Net.Sockets.SocketException || exception is System.IO.IOException)
                {

                    this.remote_server = new ServerChange(ref base.client_library).Execute();
                    if (this.remote_server != null)
                    {
                        try
                        {
                            int n_replicas = this.remote_server.Hello(this.client_identifier, this.client_remoting, this.client_ip, this.client_port);
                            base.client_library.NReplicas = n_replicas;
                        }
                        catch (System.Net.Sockets.SocketException)
                        {
                            Console.WriteLine("We cannot find anymore servers to connect to! Aborting...");
                            CrashClientProcess();
                        }
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("We cannot find anymore servers to connect to! Aborting...");
                        CrashClientProcess();
                    }
                }
            }
            
            return null;
        }

    }
}
