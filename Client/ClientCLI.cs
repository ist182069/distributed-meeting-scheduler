using MSDAD.Client.Commands;
using MSDAD.Client.Commands.CLI;
using MSDAD.Client.Exceptions;
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
            const string PING_COMMAND = "ping";
            const string CREATE = "create";
            const string EXIT = "exit";
            const string LIST = "list";
            const string JOIN = "join";
            const string CLOSE = "close";

            public void Display()
            {
                ClientLibrary clientLibrary;               
                int port_int;
                string input, user_identifier, ip_string, port_string, server_identifier;

                ip_string = ClientUtils.GetLocalIPAddress();
                Console.Write("Pick a client port: ");
                port_string = Console.ReadLine();
                port_int = Int32.Parse(port_string);
                // TODO adicionar excepcao aqui

                Console.Write("Pick a user identifier: ");
                user_identifier = Console.ReadLine();

                Console.Write("Type the server identifier to whom you want to connect: ");
                server_identifier = Console.ReadLine();

                Console.WriteLine("Your current Meeting Scheduler IP and port combination is: \"" + ip_string + ":" + port_string + "/" + user_identifier);
                clientLibrary = new ClientLibrary(user_identifier, server_identifier, ip_string, port_int);
                // Serve apenas para inicializar, caso contrario temos de esperar por um comando para registar no servidor
                new Initialize(ref clientLibrary);


                while (true)
                {                    
                    Console.Write("Insert the command you want to run on the Meeting Scheduler: ");
                    input = Console.ReadLine();                    

                    try
                    {
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
                                Console.WriteLine(ErrorCodes.INVALID_COMMAND);
                                break;
                        }
                    } catch(Exception exception) when (exception is ClientLocalException || exception is ServerCoreException)
                    {
                        Console.WriteLine(exception.Message);
                    } 
                    
                }
            }
        }
    }
}
