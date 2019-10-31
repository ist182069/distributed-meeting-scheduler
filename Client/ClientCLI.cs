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
            public const string CLOSE = "close";

            public void Display()
            {
                ClientLibrary clientLibrary;               
                int port_int;
                string input, ip_string, port_string;

                ip_string = ClientUtils.GetLocalIPAddress();
                Console.Write("Pick a client port: ");
                port_string = Console.ReadLine();

                // TODO adicionar excepcao aqui
                port_int = Int32.Parse(port_string);

                Console.WriteLine("Your current Meeting Scheduler IP and port combination is: \"" + ip_string + ":" + port_string);
                clientLibrary = new ClientLibrary(ip_string, port_int);
                // Serve apenas para inicializar, caso contrario temos de esperar por um comando para registar no servidor
                new Initialize(ref clientLibrary);


                while (true)
                {                    
                    Console.Write("Insert the command you want to run on the Meeting Scheduler: ");
                    input = Console.ReadLine();                    

                    switch (input)
                    {
                        case PING_COMMAND:
                            new Ping(ref clientLibrary).Execute();                                            
                            break;
                        case CREATE:
                            new Create(ref clientLibrary).Execute();
                            break;
                        case LIST:
                            new List(ref clientLibrary).Execute();
                            break;
                        case JOIN:
                            new Join(ref clientLibrary).Execute();
                            break;
                        case CLOSE:
                            new Close(ref clientLibrary).Execute();
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
