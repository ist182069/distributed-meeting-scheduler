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

                Communication serverCommunication;
                serverCommunication = new Communication();
                serverCommunication.Start("11000");
                Console.WriteLine("the server has been successfully started!");

                Console.ReadLine();
            }
        }
    }
}
