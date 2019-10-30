using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Client.Commands
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

            this.server.Ping(this.ip, this.port, message);

            return null;
        }
    }
}
