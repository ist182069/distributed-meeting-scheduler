using System;
using System.Threading;

namespace MSDAD.Server
{
    class ServerMain
    {
        static void Main(string[] args)
        {
            ServerParser serverParser;

            if (args.Length == 0)
            {                
                serverParser = new ServerParser();
                serverParser.Execute();
            }
            else if (args.Length == 2)
            {
                string server_identifier, server_url;

                server_identifier = args[0];
                server_url = args[1];

                serverParser = new ServerParser(server_identifier, server_url);
                serverParser.Execute();
            }
                
        }
    }    
}
