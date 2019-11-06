using System;

namespace MSDAD.Server
{
    class ServerCLI
    {
        public const string EXIT = "exit";
        public void Display()
        {
            int port_int;
            string command, server_identifier, ip_string, port_string;
            ServerCommunication serverCommunication;

            ip_string = ServerUtils.GetLocalIPAddress();
            Console.Write("Pick a server port: ");
            port_string = Console.ReadLine();
            port_int = Int32.Parse(port_string);
            // TODO adicionar excepcao aqui

            Console.Write("Type the server identifier: ");
            server_identifier = Console.ReadLine();

            serverCommunication = new ServerCommunication(server_identifier, ip_string, 11000);
            // 11000 para ser substituido pelo port_string quando o MASTER OF PUPPETS estiver feito!

            serverCommunication.Start();

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

