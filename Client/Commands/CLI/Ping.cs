using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Client.Commands.CLI

{
    class Ping : Command
    {
        public Ping(ref ClientLibrary clientLibrary) : base(ref clientLibrary)
        {

        }
        public override object Execute()
        {
            string message;

            message = "ping";

            this.server.Ping(message, this.user);

            return null;
        }
    }
}
