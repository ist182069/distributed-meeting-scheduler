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

        public Close(ref ClientLibrary clientLibrary, string[] words) : base(ref clientLibrary)
        {
            this.words = words;
        }

        public override object Execute()
        {
            string topic;

            topic = this.words[1];

            this.server.Close(topic, this.ip, this.port);

            return null;
        }

    }
}
