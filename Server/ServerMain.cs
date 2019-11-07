using System;

namespace MSDAD.Server
{
    class ServerMain
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                ServerCLI serverCLI;
                serverCLI = new ServerCLI();
                serverCLI.Display();
            }
            else if (args.Length == 1)
            {
                ServerParser serverParser;
                serverParser = new ServerParser(args[0]);
                serverParser.Execute();
            }
                
        }
    }    
}
