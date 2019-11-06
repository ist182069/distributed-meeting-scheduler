using MSDAD.Library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.PCS
{
    class RemotePCS : MarshalByRefObject, PCSInterface
    {
        PCSParser parser;

        public RemotePCS(PCSParser parser)
        {
            this.parser = parser;
        }
        public void Send(string text)
        {            
            // assuming the PCS always receives commands
            this.parser.Execute(text);
        }
    }
}
