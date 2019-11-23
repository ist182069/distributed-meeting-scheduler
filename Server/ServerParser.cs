using MSDAD.Server.Commands;
using System;

namespace MSDAD.Server
{
    class ServerParser
    {
        public const string ADD_ROOM = "AddRoom";
        public const string EXIT = "exit";
        public const string STATUS = "Status";

        private string server_identifier;
        private string server_url;
        private int tolerated_faults;
        private int min_delay;
        private int max_delay;
        
        public ServerParser()
        {
            this.server_identifier = null;
            this.server_url = null;
            this.tolerated_faults = 0;
            this.min_delay = 0;
            this.max_delay = 0;
        }
        public ServerParser(string server_identifier, string server_url, string tolerated_faults, string min_delay, string max_delay)
        {
            this.server_identifier = server_identifier;
            this.server_url = server_url;
            this.tolerated_faults = Int32.Parse(tolerated_faults);
            this.min_delay = Int32.Parse(min_delay);
            this.max_delay = Int32.Parse(max_delay);
        }
        public void Execute()
        {
            ServerLibrary server_library;
            int server_port = 0;
            string command, server_arguments, server_ip, server_remoting;
            string[] split_command;

            if(server_url == null && server_identifier == null)
            {
                Console.Write("Type the server parameters within the following format \"s1 tcp://192.168.1.165:8081/server1 0 100 200\": ");
                server_arguments = Console.ReadLine();
                split_command = server_arguments.Split(' ');

                this.server_identifier = split_command[0];
                this.server_url = split_command[1];
                this.tolerated_faults = Int32.Parse(split_command[2]);
                this.min_delay = Int32.Parse(split_command[3]);
                this.max_delay = Int32.Parse(split_command[4]);              
            }

            server_port = ServerUtils.GetPortFromUrl(this.server_url);            
            server_ip = ServerUtils.GetIPFromUrl(this.server_url);
            server_remoting = ServerUtils.GetRemotingIdFromUrl(this.server_url);

            // Serve apenas para inicializar, caso contrario temos de esperar por um comando para registar no servidor

            Console.Write("Starting up server... ");
            server_library = new ServerLibrary(this.server_identifier, server_remoting, server_ip, server_port, tolerated_faults, min_delay, max_delay);
            new Initialize(ref server_library);
            Console.WriteLine("the server has been successfully started: tcp://" + server_ip + ":" + server_port + "/" + server_remoting);

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
                    case STATUS:
                        new Status(ref server_library).Execute();
                        break;
                    default:
                        Console.WriteLine("That command does not exist. You must insert another one");
                        break;
                }
            }
        }
    }
}

