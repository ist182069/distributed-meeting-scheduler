using System;

namespace MSDAD
{
    namespace Server
    {
        class ServerCLI
        {
            public void Display()
            {
                Console.Write("Starting up server...");

                ServerCommunication serverCommunication;
                serverCommunication = new ServerCommunication();
                serverCommunication.Start("11000");
                Console.WriteLine("the server has been successfully started!");

                Console.ReadLine();
            }
        }
    }
}
