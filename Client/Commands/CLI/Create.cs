using MSDAD.Client.Commands;
using MSDAD.Client.Comunication;
using MSDAD.Client.Exceptions;
using MSDAD.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Client.Commands.CLI

{
    class Create : Command
    {
        public Create(ref ClientLibrary clientLibrary) : base(ref clientLibrary)
        {
        }
        public override object Execute()
        {                                  
            int minAttendees, num_slots, num_invitees;
            string client_string, invitee_address, room, topic;
            MeetingView meetingView;

            List<string> invitees = new List<string>();

            Console.WriteLine("Insert the following parameters");
            Console.WriteLine("Meeting topic: ");
            topic = Console.ReadLine();

            Console.WriteLine("Minimum Attendees: ");
            minAttendees = Int32.Parse(Console.ReadLine());

            Console.WriteLine("Number of slots: ");
            num_slots = Int32.Parse(Console.ReadLine());

            Console.WriteLine("Number of invitees: ");
            num_invitees = Int32.Parse(Console.ReadLine());

            Console.WriteLine("Insert slots of the type \"Lisboa,2020-01-02\":\n");
            List<string> slots = new List<string>();
            for (int i = 0; i<num_slots; i++)
            {
                room = Console.ReadLine();

                if (slots.Contains(room))
                {
                    throw new ClientLocalException("Create.Execute(): You cannot add the same room twice to the rooms list! Aborting...");
                }
                slots.Add(room);
            }
           
            if (num_invitees==0)
            {
                this.server.Create(topic, minAttendees, slots, null, this.user);
                meetingView = new MeetingView(topic, 1, "OPEN");
                this.clientLibrary.AddMeetingView(meetingView);
            }
            else
            {
                Console.WriteLine("Insert the user identifier of the invitees:\n");

                for (int i = 0; i<num_invitees; i++)
                {
                    invitee_address = Console.ReadLine();

                    if (invitees.Contains(invitee_address))
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

                this.server.Create(topic, minAttendees, slots, invitees, this.user);
            }
                    
            return null;
        }
    }
}
