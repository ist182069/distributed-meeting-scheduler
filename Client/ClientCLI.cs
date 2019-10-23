using MSDAD.Client.Commands;
using MSDAD.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD
{
    namespace Client
    {
        class ClientCLI
        {
            public const string PING_COMMAND = "ping";
            public const string CREATE = "create";
            public const string EXIT = "exit";
            public const string LIST = "list";
            public const string JOIN = "join";

            public void Display()
            {
                
                ClientLibrary clientLibrary;               
                int port_int;
                string command, port_string;

                Console.Write("Pick a client port: ");
                port_string = Console.ReadLine();

                // TODO adicionar excepcao aqui
                port_int = Int32.Parse(port_string);

                clientLibrary = new ClientLibrary(port_int);

                while (true)
                {                    
                    Console.Write("Insert the command you want to run on the Meeting Scheduler: ");
                    command = Console.ReadLine();                    

                    switch (command)
                    {
                        case PING_COMMAND:                            
                            clientLibrary.Ping();                            
                            break;
                        case CREATE:
                            clientLibrary.Create();
                            break;
                        case LIST:
                            clientLibrary.List();
                            break;
                        case JOIN:
                            clientLibrary.Join();
                            break;
                        case EXIT:
                            Console.WriteLine("Bye!");
                            return;
                        default:
                            Console.WriteLine("You must insert a valid command");
                            break;
                    }
                }
            }
        }
    }
}
