using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD
{
    namespace Client
    {
        public const string PING_COMMAND = "ping";

        class ClientUI
        {
            public void Display()
            {
                string command; 

                while (true)
                {
                    Console.Write("Insert the command you want to run on the Meeting Scheduler: ");
                    command = Console.ReadLine();

                    switch (command)
                    {
                        case PING_COMMAND:
                            this.server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), "tcp://localhost:11000/RemoteServer");
                            break;
                        default:
                            Console.WriteLine("You must insert a valid command");
                            break;
                    }
                }                
        }
    }
}
