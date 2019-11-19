using MSDAD.Server.Commands;
using System;

namespace MSDAD.Server
{
    class ServerParser
    {
        public const string ADD_ROOM = "AddRoom";
        public const string EXIT = "exit";

        private string server_url;

        public ServerParser()
        {
            server_url = null;
        }
        public ServerParser(string server_url)
        {
            this.server_url = server_url;
        }
        public void Execute()
        {
            ServerLibrary server_library;
            int server_port = 0;
            string command, server_identifier, server_ip, server_port_string = "";
            string[] split_command;

            server_ip = ServerUtils.GetLocalIPAddress();

            bool server_port_is_correct = false;

            if(server_url == null)
            {
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
            }

            else
            {
                server_port = ServerUtils.GetPortFromUrl(this.server_url);
                server_ip = ServerUtils.GetIPFromUrl(this.server_url);
                server_identifier = ServerUtils.GetIdFromUrl(this.server_url);    
            }

            // Serve apenas para inicializar, caso contrario temos de esperar por um comando para registar no servidor

            Console.Write("Starting up server... ");
            server_library = new ServerLibrary(server_identifier, server_ip, server_port);
            new Initialize(ref server_library);
            Console.WriteLine("the server has been successfully started: tcp://" + server_ip + ":" + server_port_string + "/" + server_identifier);

            while (true)
            {

                Console.Write("Please run a command to be run on the server: ");
                command = Console.ReadLine();

                split_command = command.Split(' ');

                switch (split_command[0])
                {
                    case ADD_ROOM:
                        new AddRoom(ref server_library, split_command).Execute();
                        break;
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

