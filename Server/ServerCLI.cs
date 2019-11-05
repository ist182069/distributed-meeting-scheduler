using System;

namespace MSDAD.Server
{
    class ServerCLI
    {
        public const string EXIT = "exit";
        public void Display()
        {
            string command;
            ServerCommunication serverCommunication;
                
            Console.Write("Starting up server...");
                
            serverCommunication = new ServerCommunication();
            serverCommunication.Start("11000");

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

