using MSDAD.Client.Commands;
using MSDAD.Client.Comunication;
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
        namespace Commands
        {
            class Create : Command
            {
                public Create(ref ClientLibrary clientLibrary) : base(ref clientLibrary)
                {
                }
                public override object Execute()
                {                                  
                    int minAttendees;
                    string client_string, portInvitee, room, topic;
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
                    try
                    {
                        if (Console.ReadLine().Equals("n"))
                        {
                            this.server.Create(topic, minAttendees, slots, null, this.ip, this.port);


                        }
                        else
                        {
                            Console.WriteLine("Write invitees port, then type end \n");

                            while (!(portInvitee = Console.ReadLine()).Equals("end"))
                            {
                                invitees.Add(Int32.Parse(portInvitee));
                            }

                            this.server.Create(topic, minAttendees, slots, invitees, this.ip, this.port);
                        }
                    } catch (ServerCommunicationException e) {
                        Console.WriteLine(e.Message);
                    }

                    client_string = ClientUtils.AssembleClientAddress(this.ip, this.port);
    
                    meetingView = new MeetingView(topic, null, client_string, 1, "OPEN");

                    this.clientLibrary.AddMeetingView(meetingView);

                    return null;
                }
            }
        }
    }
}
