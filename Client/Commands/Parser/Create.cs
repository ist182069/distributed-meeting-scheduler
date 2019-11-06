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

        public Create(ref ClientLibrary clientLibrary, string[] words) : base(ref clientLibrary)
        {
            this.words = words;
        }

        public override object Execute()
        {

            int min_attendees, num_slots, num_invitees;
            string invitee_address, room, topic;

            MeetingView meetingView;
            List<string> slots = new List<string>(), invitees;

            topic = this.words[1];
            min_attendees = Int32.Parse(this.words[2]);
            num_slots = Int32.Parse(this.words[3]);
            num_invitees = Int32.Parse(this.words[4]);                        

            for(int i = 5; i <= (num_slots+4); i++)
            {
                room = this.words[i];
                
                if (slots.Contains(room))
                {
                    throw new ClientLocalException("Error! You cannot add the same room twice to the rooms list! Aborting...");
                }
                slots.Add(room);                
            }

            if(num_invitees==0)
            {
                this.server.Create(topic, min_attendees, slots, null, this.ip, this.port);
                meetingView = new MeetingView(topic, 1, "OPEN");
                this.clientLibrary.AddMeetingView(meetingView);
            }
            else
            {
                invitees = new List<string>();
                for (int i = (num_slots + 4) + 1 ; i < this.words.Length; i++)
                {
                    invitee_address = this.words[i];

                    if (invitees.Contains(invitee_address))
                    {
                        throw new ClientLocalException("Create.Execute(): You cannot add the same invitee twice to the rooms list! Aborting...");
                    }

                    if (invitee_address == this.client_address)
                    {
                        meetingView = new MeetingView(topic, 1, "OPEN");
                        this.clientLibrary.AddMeetingView(meetingView);
                    }

                    invitees.Add(invitee_address);

                    this.server.Create(topic, min_attendees, slots, invitees, this.ip, this.port);
                }
            }
                
            return null;
        }
    }
}
