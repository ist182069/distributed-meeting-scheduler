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
            public const string LIST = "list";
            public void Display()
            {
                Command commandClass;
                ClientCommunication clientCommunication;
                ServerInterface server;
                int port_int;
                string command, message, port_string;

                Console.Write("Pick a client port: ");
                port_string = Console.ReadLine();

                // TODO adicionar excepcao aqui
                port_int = Int32.Parse(port_string);

                clientCommunication = new ClientCommunication();
                clientCommunication.Start(port_int);

                while (true)
                {
                    clientCommunication.GetRemoteServer();
                    clientCommunication.Hello(port_int);

                    Console.Write("Insert the command you want to run on the Meeting Scheduler: ");
                    command = Console.ReadLine();                    

                    switch (command)
                    {
                        case PING_COMMAND:                            
                            clientCommunication.Ping();                            
                            break;
                        case CREATE:
                            commandClass = new Create();
                            commandClass.Execute(clientCommunication, port_int);
                            break;
                        case LIST:
                            string listData = clientCommunication.List(port_int);
                            Console.WriteLine(listData);
                            break;
                        default:
                            Console.WriteLine("You must insert a valid command");
                            break;
                    }
                }
            }
        }
    }
}
