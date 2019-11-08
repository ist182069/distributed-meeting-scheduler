using MSDAD.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Client.Commands

{
    class Initialize : Command
    {
        public Initialize(ref ClientLibrary client_library) : base(ref client_library)
        {
        }
        public override object Execute()
        {            
            try
            {
                this.remote_server.Hello(this.client_identifier, this.client_ip, this.client_port);
                return null;
            }
            catch (ServerCoreException e)
            {
                client_library.ClientCommunication.Destroy();
                throw e;
            }
        }
    }
}
