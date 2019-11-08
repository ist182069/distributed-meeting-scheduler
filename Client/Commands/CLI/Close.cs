using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSDAD.Client.Comunication;
using MSDAD.Library;

namespace MSDAD.Client.Commands.CLI
{
    class Close : Command
    {
        public Close(ref ClientLibrary client_library) : base(ref client_library)
        {

        }
        public override object Execute()
        {
            string meeting_topic;

            Console.WriteLine("Write meeting topic:");
            meeting_topic = Console.ReadLine();


            this.remote_server.Close(meeting_topic, this.client_identifier, 0);
            Console.WriteLine("Successfully scheduled " + meeting_topic);

            return null;
        }
    }
}
