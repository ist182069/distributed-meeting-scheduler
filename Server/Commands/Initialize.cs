using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Server.Commands

{
    class Initialize : Command
    {
        public Initialize(ref ServerLibrary serverLibrary) : base(ref serverLibrary)
        {

        }
        public override object Execute()
        {           
            return null;
        }
    }
}
