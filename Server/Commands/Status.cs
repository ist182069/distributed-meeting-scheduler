using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Server.Commands
{
    class Status : Command
    {
        public Status(ref ServerLibrary server_library) : base(ref server_library)
        {            
        }
        public override object Execute()
        {
            base.server_library.Status();

            return null;
        }
    }
}
