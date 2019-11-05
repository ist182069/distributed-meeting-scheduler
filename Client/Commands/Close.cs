using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSDAD.Client.Comunication;
using MSDAD.Library;

namespace MSDAD.Client.Commands
{
    class Close : Command
    {
        public Close(ref ClientLibrary clientLibrary) : base(ref clientLibrary)
        {

        }
        public override object Execute()
        {
            string topic;

            Console.WriteLine("Write meeting topic:");
            topic = Console.ReadLine();


            this.server.Close(topic, ip, port);
            Console.WriteLine("Successfully scheduled " + topic);

            return null;
        }
    }
}
