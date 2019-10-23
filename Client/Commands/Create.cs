using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD
{
    namespace Client
    {
        namespace Commands
        {
            class Create : Command
            {
                public override void Execute(ClientCommunication clientCommunication, int port_int)
                {
                    Console.WriteLine("Insert the following parameters");
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

                    }
                    else
                    {
                        Console.WriteLine("Write invitees port, then type end \n");
                        List<int> invitees = new List<int>();
                        string portInvitee;
                        while (!(portInvitee = Console.ReadLine()).Equals("end"))
                        {
                            invitees.Add(Int32.Parse(portInvitee));
                        }
                        clientCommunication.Create(topic, minAttendees, slots, invitees, port_int);
                    }
                }
            }
        }
    }
}
