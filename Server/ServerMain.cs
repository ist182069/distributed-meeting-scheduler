using System;
using System.Threading;

namespace MSDAD.Server
{
    class ServerMain
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                ServerCLI server_CLI;
                server_CLI = new ServerCLI();
                server_CLI.Display();
            }
            else if (args.Length == 1)
            {
                ServerParser server_parser;
                server_parser = new ServerParser(args[0]);
                server_parser.Execute();
            }
                
        }
    }    
}
