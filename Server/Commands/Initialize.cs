using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Server.Commands

{
    class Initialize : Command
    {
        public Initialize(ref ServerLibrary server_library) : base(ref server_library)
        {

        }
        public override object Execute()
        {           
            return null;
        }
    }
}
