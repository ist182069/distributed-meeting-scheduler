using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Server
{
    class ServerParser
    {
        string server_url;

        public ServerParser(string server_url)
        {
            this.server_url = server_url;
        }
        public void Execute()
        {
            int port_int;
            string server_identifier, ip_string;
            ServerCommunication serverCommunication;

            port_int = ServerUtils.GetPortFromUrl(this.server_url);
            ip_string = ServerUtils.GetIPFromUrl(this.server_url);
            server_identifier = ServerUtils.GetServerIdFromUrl(this.server_url);

            Console.Write("Starting up server... ");
            serverCommunication = new ServerCommunication(server_identifier, ip_string, port_int);
            serverCommunication.Start();            
            Console.WriteLine("Success!");

            while (true) ;
        }
    }
}
