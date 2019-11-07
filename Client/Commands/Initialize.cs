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
            return null;
        }
    }
}
