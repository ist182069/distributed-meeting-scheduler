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
            string room, topic;
            List<string> slots;

            Console.WriteLine("Write meeting topic:");
            topic = Console.ReadLine();
            Console.WriteLine("Write slots of the type Lisboa,2020-01-02 you can attend, then type end:");
            slots = new List<string>();

            while (!(room = Console.ReadLine()).Equals("end"))
            {
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
