using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Client.Commands.Parser
{
    class Close : Command
    {
        string[] words;

        public Close(ref ClientLibrary client_library, string[] words) : base(ref client_library)
        {
            this.words = words;
        }

        public override object Execute()
        {
            string topic;

            topic = this.words[1];

            this.remote_server.Close(topic, this.client_identifier);

            return null;
        }

    }
}
