using MSDAD.Client.Commands;
using MSDAD.Client.Comunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD
{
    namespace Client
    {
        namespace Library
        {
            class Create : Command
            {
                public override object Execute(ClientSendComm comm, int port_int)
                {
                    
                    int minAttendees;
                    string portInvitee, room, topic;
                    MeetingView meetingView;

                    List<int> invitees = new List<int>();

                    Console.WriteLine("Insert the following parameters");
                    Console.WriteLine("Meeting topic: ");
                    topic = Console.ReadLine();

                    Console.WriteLine("Minimum Attendees: ");
                    minAttendees = Int32.Parse(Console.ReadLine());

                    Console.WriteLine("Write slots of the type Lisboa,2020-01-02, then type end:");
                    List<string> slots = new List<string>();
                    while (!(room = Console.ReadLine()).Equals("end"))
                    {
                        slots.Add(room);
                    }

                    Console.WriteLine("Want invitees? y:n");
                    if (Console.ReadLine().Equals("n"))
                    {
                        comm.Create(topic, minAttendees, slots, null, port_int);

                    }
                    else
                    {
                        Console.WriteLine("Write invitees port, then type end \n");
                        
                        while (!(portInvitee = Console.ReadLine()).Equals("end"))
                        {
                            invitees.Add(Int32.Parse(portInvitee));
                        }
                        comm.Create(topic, minAttendees, slots, invitees, port_int);                        
                    }
                    meetingView = new MeetingView(topic, slots, port_int);

                    return meetingView;
                }
            }
        }
    }
}
