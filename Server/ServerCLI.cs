using MSDAD.Server.Commands;
using System;

namespace MSDAD.Server
{
    class ServerCLI
    {
        public const string EXIT = "exit";
        public void Display()
        {
            ServerLibrary server_library;
            int server_port = 0;
            string command, server_identifier, server_ip, server_port_string = "";

            server_ip = ServerUtils.GetLocalIPAddress();

            bool server_port_is_correct = false;
            while (!server_port_is_correct)
            {
                try
                {
                    Console.Write("Pick a server port: ");
                    server_port_string = Console.ReadLine();
                    server_port = Int32.Parse(server_port_string);
                    server_port_is_correct = true;
                }
                catch (FormatException)
                {
                    Console.WriteLine(ErrorCodes.INVALID_PORT_FORMAT);
                }
            }

            Console.Write("Type the server identifier: ");
            server_identifier = Console.ReadLine();

            server_library = new ServerLibrary(server_identifier, server_ip, server_port);
            // Serve apenas para inicializar, caso contrario temos de esperar por um comando para registar no servidor
            new Initialize(ref server_library);

            Console.WriteLine("the server has been successfully started: tcp://" + server_ip + ":" + server_port_string + "/" + server_identifier);

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

