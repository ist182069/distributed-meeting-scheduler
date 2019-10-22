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
            public void Display()
            {
                Communication clientCommunication;
                ServerInterface server;
                int port_int;
                string command, message, port_string;

                Console.Write("Pick a client port: ");
                port_string = Console.ReadLine();

                // TODO adicionar excepcao aqui
                port_int = Int32.Parse(port_string);

                clientCommunication = new Communication();
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
                        default:
                            Console.WriteLine("You must insert a valid command");
                            break;
                    }
                }
            }
        }
    }
}
