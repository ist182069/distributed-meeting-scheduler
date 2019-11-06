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
    class Join : Command
    {
        string[] words;

        public Join(ref ClientLibrary clientLibrary, string[] words) : base(ref clientLibrary)
        {
            this.words = words;
        }

        public override object Execute()
        {

            int num_slots;
            string room, topic;

            List<string> slots = new List<string>();

            topic = this.words[1];
            num_slots = Int32.Parse(this.words[2]);

            for (int i = 3; i < (num_slots)+3; i++)
            {
                Console.WriteLine();
                room = this.words[i];

                if (slots.Contains(room))
                {
                    throw new ClientLocalException(ErrorCodes.DUPLICATED_SLOT);
                }
                
                slots.Add(room);
            }

            this.server.Join(topic, slots, this.user);

            return null;
        }
    }
}
