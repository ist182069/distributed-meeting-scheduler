using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSDAD.Client.Comunication;
using MSDAD.Library;

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
                slots.Add(room);
            }

            try
            {
                this.server.Join(topic, slots, this.port);
                Console.WriteLine("Registered in " + topic);
            } catch (ServerCommunicationException e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }
    }
}
