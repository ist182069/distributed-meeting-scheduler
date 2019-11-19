using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Client.Commands

{
    class Ping : Command
    {
        public Ping(ref ClientLibrary client_library) : base(ref client_library)
        {

        }
        public override object Execute()
        {
            string message;

            message = "ping";

            this.remote_server.Ping(message, this.client_identifier);

            return null;
        }
    }
}
