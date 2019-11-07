using MSDAD.Server.Commands;
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
            ServerLibrary serverLibrary;
            int port_int;
            string server_identifier, ip_string;
            
            port_int = ServerUtils.GetPortFromUrl(this.server_url);
            ip_string = ServerUtils.GetIPFromUrl(this.server_url);
            server_identifier = ServerUtils.GetIdFromUrl(this.server_url);

            Console.Write("Starting up server... ");
            serverLibrary = new ServerLibrary(server_identifier, ip_string, 11000);
            new Initialize(ref serverLibrary);
            Console.WriteLine("Success!");

            while (true) ;
        }
    }
}
