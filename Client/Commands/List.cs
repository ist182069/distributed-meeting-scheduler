using MSDAD.Client.Comunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Client.Commands
{
    class List : Command
    {
        public override object Execute(ClientSendComm comm, int port_int)
        {
            comm.List(port_int);

            // TODO ira processar o estado que ira receber do servidor

            return null;
        }
    }
}
