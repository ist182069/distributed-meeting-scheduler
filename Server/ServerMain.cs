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
            else if (args.Length == 5)
            {
                string server_identifier, server_url, tolerated_faults, min_delay, max_delay;

                server_identifier = args[0];
                server_url = args[1];
                tolerated_faults = args[2];
                min_delay = args[3];
                max_delay = args[4];

                serverParser = new ServerParser(server_identifier, server_url, tolerated_faults, min_delay, max_delay);
                serverParser.Execute();
            }
                
        }
    }    
}
