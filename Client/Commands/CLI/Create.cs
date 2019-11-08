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
        public Create(ref ClientLibrary client_library) : base(ref client_library)
        {
        }
        public override object Execute()
        {                                  
            int min_attendees, num_slots, num_invitees;
            string invitee_address, slot, meeting_topic;
            MeetingView meeting_view;

            List<string> invitees = new List<string>();

            Console.WriteLine("Insert the following parameters");
            Console.WriteLine("Meeting topic: ");
            meeting_topic = Console.ReadLine();

            Console.WriteLine("Minimum Attendees: ");
            try
            {
                min_attendees = Int32.Parse(Console.ReadLine());
            }
            catch(FormatException e)
            {
                throw new ClientLocalException(ErrorCodes.INVALID_MIN_ATTENDES);
            }

            if (min_attendees < 1)
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
                slot = Console.ReadLine();

                if (slots.Contains(slot))
                {
                    throw new ClientLocalException(ErrorCodes.DUPLICATED_SLOT);
                }
                slots.Add(slot);
            }
           
            if (num_invitees==0)
            {
                this.remote_server.Create(meeting_topic, min_attendees, slots, null, this.client_identifier, 0);
                meeting_view = new MeetingView(meeting_topic, 1, "OPEN");
                this.client_library.AddMeetingView(meeting_view);
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

                    if(invitee_address == this.client_identifier)
                    {
                        meeting_view = new MeetingView(meeting_topic, 1, "OPEN");
                        this.client_library.AddMeetingView(meeting_view);
                    }
                    invitees.Add(invitee_address);
                }

                this.remote_server.Create(meeting_topic, min_attendees, slots, invitees, this.client_identifier, 0);
            }
                    
            return null;
        }
    }
}
