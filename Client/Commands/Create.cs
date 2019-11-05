using MSDAD.Client.Commands;
using MSDAD.Client.Comunication;
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
                    string client_string, invitee_address, room, topic;
                    MeetingView meetingView;

                    List<string> invitees = new List<string>();

                    Console.WriteLine("Insert the following parameters");
                    Console.WriteLine("Meeting topic: ");
                    topic = Console.ReadLine();

                    Console.WriteLine("Minimum Attendees: ");
                    minAttendees = Int32.Parse(Console.ReadLine());

                    Console.WriteLine("Write slots of the type Lisboa,2020-01-02, then type end:");
                    List<string> slots = new List<string>();
                    while (!(room = Console.ReadLine()).Equals("end"))
                    {
                        if(slots.Contains(room))
                        {
                            throw new ClientLocalException("Create.Execute(): You cannot add the same room twice to the rooms list! Aborting...");
                        }
                        slots.Add(room);
                    }

                    Console.WriteLine("Want invitees? y:n");

                    if (Console.ReadLine().Equals("n"))
                    {
                        this.server.Create(topic, minAttendees, slots, null, this.ip, this.port);
                        meetingView = new MeetingView(topic, 1, "OPEN");
                        this.clientLibrary.AddMeetingView(meetingView);
                    }
                    else
                    {
                        Console.WriteLine("Write invitees of the type ip:port, then type end \n");

                        while (!(invitee_address = Console.ReadLine()).Equals("end"))
                        {
                            if(invitees.Contains(invitee_address))
                            {
                                throw new ClientLocalException("Create.Execute(): You cannot add the same invitee twice to the rooms list! Aborting...");
                            }

                            if(invitee_address == this.client_address)
                            {
                                meetingView = new MeetingView(topic, 1, "OPEN");
                                this.clientLibrary.AddMeetingView(meetingView);
                            }
                            invitees.Add(invitee_address);
                        }

                        this.server.Create(topic, minAttendees, slots, invitees, this.ip, this.port);
                    }
                    
                    return null;
                }
            }
        }
    }
}
