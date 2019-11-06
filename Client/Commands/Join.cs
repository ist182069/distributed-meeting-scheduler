using MSDAD.Client.Comunication;
using MSDAD.Client.Exceptions;
using MSDAD.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Client.Commands
{
    class Join : Command
    {
        public Join(ref ClientLibrary clientLibrary) : base(ref clientLibrary)
        {

        }
        public override object Execute()
        {
            int num_slots;
            string room, topic;
            List<string> slots;

            Console.WriteLine("Write meeting topic:");
            topic = Console.ReadLine();
            Console.WriteLine("Insert slots of the type \"Lisboa,2020-01-02\":");
            slots = new List<string>();
            Console.WriteLine("Number of slots: ");
            num_slots = Int32.Parse(Console.ReadLine());

            for(int i = 0; i<num_slots; i++)
            {
                room = Console.ReadLine();

                if (slots.Contains(room))
                {
                    throw new ClientLocalException("Create.Execute(): You cannot add the same room twice to the rooms list! Aborting...");
                }

                slots.Add(room);
            }

            this.server.Join(topic, slots, this.ip, this.port);
            Console.WriteLine("Registered in " + topic);

            return null;
        }
    }
}
