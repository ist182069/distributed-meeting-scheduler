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
            // isto e feito para nao violar as layers de abstracao e nao existir codigo duplicado. Caso contrario as mesmas linhas eram usadas tanto no ServerCommunications como nesta classe
            // a outra estrategia era o estado ser listado na propria library. Mas como nos estamos a fazer no codigo a library nao faz print de informacao
            ServerUtils.Status(this.server_library);

            return null;
        }
    }
}
