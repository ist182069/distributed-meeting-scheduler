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
    class Join : Command
    {
        public Join(ref ClientLibrary clientLibrary) : base(ref clientLibrary)
        {

        }
        public override object Execute()
        {
            int num_slots;
            string slot, meeting_topic;
            List<string> slots;

            Console.WriteLine("Write meeting topic:");
            meeting_topic = Console.ReadLine();
            
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

            Console.WriteLine("Insert slots of the type \"Lisboa,2020-01-02\":");
            slots = new List<string>();
            for (int i = 0; i<num_slots; i++)
            {
                slot = Console.ReadLine();

                if (slots.Contains(slot))
                {
                    throw new ClientLocalException(ErrorCodes.DUPLICATED_SLOT);
                }

                slots.Add(slot);
            }

            this.remote_server.Join(meeting_topic, slots, this.client_identifier);
            Console.WriteLine("Registered in " + meeting_topic);

            return null;
        }
    }
}
