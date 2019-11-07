using System;

namespace MSDAD.Server
{
    class ServerMain
    {
        static void Main(string[] args)
        {
            ServerCLI serverCLI;

            serverCLI = new ServerCLI();
            serverCLI.Display();
        }
    }    
}
