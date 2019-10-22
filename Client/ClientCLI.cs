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
                string command, message, port;

                Console.Write("Pick a client port: ");
                port = Console.ReadLine();

                clientCommunication = new Communication();
                clientCommunication.Start(port);

                while (true)
                {
                    Console.Write("Insert the command you want to run on the Meeting Scheduler: ");
                    command = Console.ReadLine();

                    switch (command)
                    {
                        case PING_COMMAND:
                            clientCommunication.GetRemoteServer();
                            message = clientCommunication.Ping();
                            Console.WriteLine(message);
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
