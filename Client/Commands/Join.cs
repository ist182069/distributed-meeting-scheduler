using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSDAD.Client.Comunication;

namespace MSDAD.Client.Commands
{
    class Join : Command
    {
        public override object Execute(ClientSendComm comm, int port_int)
        {
            string roomJoin, topicJoin;
            List<string> slotsJoin;

            Console.WriteLine("Write meeting topic:");
            topicJoin = Console.ReadLine();
            Console.WriteLine("Write slots of the type Lisboa,2020-01-02 you can attend, then type end:");
            slotsJoin = new List<string>();

            while (!(roomJoin = Console.ReadLine()).Equals("end"))
            {
                slotsJoin.Add(roomJoin);
            }

            comm.Join(topicJoin, slotsJoin, port_int);
            return topicJoin;
        }
    }
}
