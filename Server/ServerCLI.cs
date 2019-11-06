using System;

namespace MSDAD.Server
{
    class ServerCLI
    {
        public const string EXIT = "exit";
        public void Display()
        {
            string command, server_identifier;
            ServerCommunication serverCommunication;
                
            Console.WriteLine("Starting up server...");

            Console.Write("Type the server identifier: ");
            server_identifier = Console.ReadLine();

            serverCommunication = new ServerCommunication();
            serverCommunication.Start(server_identifier, "11000");

            Console.WriteLine("the server has been successfully started!");

            while (true)
            {

                Console.Write("Please run a command to be run on the server: ");
                command = Console.ReadLine();

                switch (command)
                {
                    case EXIT:
                        Console.Write("Bye!");
                        return;
                    default:
                        Console.WriteLine("That command does not exist. You must insert another one");
                        break;
                }
            }
        }
    }
}

