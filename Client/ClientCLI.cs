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
                            Console.WriteLine("Insert the folloing parameters");
                            Console.WriteLine("Meeting topic: ");
                            string topic = Console.ReadLine();

                            Console.WriteLine("Minimum Attendees: ");
                            int minAttendees = Int32.Parse(Console.ReadLine());

                            Console.WriteLine("Write slots of the type Lisboa,2020-01-02, then type end:"); 
                            string room;
                            List<string> slots = new List<string>();
                            while (!(room = Console.ReadLine()).Equals("end"))
                            {
                                slots.Add(room);
                            }

                            Console.WriteLine("Want invitees? y:n");
                            if (Console.ReadLine().Equals("n"))
                            {
                                clientCommunication.Create(topic, minAttendees, slots, null, port_int);
                                break;
                            }
                            Console.WriteLine("Write invitees port, then type end \n");
                            List<int> invitees = new List<int>();
                            string portInvitee;
                            while (!(portInvitee = Console.ReadLine()).Equals("end"))
                            {
                                invitees.Add(Int32.Parse(portInvitee));
                            }
                            clientCommunication.Create(topic, minAttendees, slots, invitees, port_int);
                            break;
                        case LIST:
                            string listData = clientCommunication.List();
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
