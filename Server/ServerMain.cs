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
            else if (args.Length == 1)
            {
                serverParser = new ServerParser(args[0]);
                serverParser.Execute();
            }
                
        }
    }    
}
