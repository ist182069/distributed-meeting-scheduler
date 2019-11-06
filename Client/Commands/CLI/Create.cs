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
            try
            {
                minAttendees = Int32.Parse(Console.ReadLine());
            }
            catch(FormatException e)
            {
                throw new ClientLocalException(ErrorCodes.INVALID_MIN_ATTENDES);
            }

            if (minAttendees < 1)
            {
                throw new ClientLocalException(ErrorCodes.INVALID_MIN_ATTENDES);
            }

            Console.WriteLine("Number of slots: ");
            try
            {
                num_slots = Int32.Parse(Console.ReadLine());
            }
            catch (FormatException e)
            {
                throw new ClientLocalException(ErrorCodes.INVALID_N_SLOTS);
            }

            if (num_slots < 1)
            {
                throw new ClientLocalException(ErrorCodes.INVALID_N_SLOTS);
            }

            Console.WriteLine("Number of invitees: ");
            try
            {
                num_invitees = Int32.Parse(Console.ReadLine());
            }
            catch (FormatException e)
            {
                throw new ClientLocalException(ErrorCodes.INVALID_N_INVITEES);
            }

            if (num_invitees < 0)
            {
                throw new ClientLocalException(ErrorCodes.INVALID_N_INVITEES);
            }
            

            Console.WriteLine("Insert slots of the type \"Lisboa,2020-01-02\":");
            List<string> slots = new List<string>();
            for (int i = 0; i<num_slots; i++)
            {
                room = Console.ReadLine();

                if (slots.Contains(room))
                {
                    throw new ClientLocalException(ErrorCodes.DUPLICATED_SLOT);
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
                Console.WriteLine("Insert the user identifier of the invitees:");

                for (int i = 0; i<num_invitees; i++)
                {
                    invitee_address = Console.ReadLine();

                    if (invitees.Contains(invitee_address))
                    {
                        throw new ClientLocalException(ErrorCodes.DUPLICATED_INVITEE);
                    }

                    if(invitee_address == this.user)
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
