using MSDAD.Client.Commands;
using MSDAD.Client.Commands.Parser;
using MSDAD.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Client.Commands.Parser
{
    class Create : Command
    {
        string[] words;

        public Create(ref ClientLibrary client_library, string[] words) : base(ref client_library)
        {
            this.words = words;
        }

        public override object Execute()
        {

            int min_attendees, num_slots, num_invitees;
            string invitee_address, room, meeting_topic;

            MeetingView meeting_view;
            List<string> slots = new List<string>(), invitees;

            meeting_topic = this.words[1];

            try
            {
                min_attendees = Int32.Parse(this.words[2]);
            }
            catch (FormatException e)
            {
                throw new ClientLocalException(ErrorCodes.INVALID_MIN_ATTENDES);
            }

            if (min_attendees < 1)
            {
                throw new ClientLocalException(ErrorCodes.INVALID_MIN_ATTENDES);
            }

            try
            {
                num_slots = Int32.Parse(this.words[3]);
            }
            catch (FormatException e)
            {
                throw new ClientLocalException(ErrorCodes.INVALID_N_SLOTS);
            }

            if (num_slots < 1)
            {
                throw new ClientLocalException(ErrorCodes.INVALID_N_SLOTS);
            }

            try
            {
                num_invitees = Int32.Parse(this.words[4]);
            }
            catch (FormatException e)
            {
                throw new ClientLocalException(ErrorCodes.INVALID_N_INVITEES);
            }

            if (num_invitees < 0)
            {
                throw new ClientLocalException(ErrorCodes.INVALID_N_INVITEES);
            }                        

            for(int i = 5; i <= (num_slots+4); i++)
            {
                room = this.words[i];
                
                if (slots.Contains(room))
                {
                    throw new ClientLocalException(ErrorCodes.DUPLICATED_SLOT);
                }
                slots.Add(room);                
            }

            if(num_invitees==0)
            {
                this.remote_server.Create(meeting_topic, min_attendees, slots, null, this.client_identifier);
                meeting_view = new MeetingView(meeting_topic, 1, "OPEN", null);
                this.client_library.AddMeetingView(meeting_view);
            }
            else
            {
                invitees = new List<string>();
                for (int i = (num_slots + 4) + 1 ; i < this.words.Length; i++)
                {
                    invitee_address = this.words[i];

                    if (invitees.Contains(invitee_address))
                    {
                        throw new ClientLocalException(ErrorCodes.DUPLICATED_SLOT);
                    }

                    if (invitee_address == this.client_address)
                    {
                        meeting_view = new MeetingView(meeting_topic, 1, "OPEN", null);
                        this.client_library.AddMeetingView(meeting_view);
                    }

                    invitees.Add(invitee_address);
                }

                this.remote_server.Create(meeting_topic, min_attendees, slots, invitees, this.client_identifier);
            }
                
            return null;
        }
    }
}
