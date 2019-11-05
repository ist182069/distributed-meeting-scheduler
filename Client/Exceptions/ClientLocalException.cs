using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Client.Exceptions
{
    class ClientLocalException : Exception
    {
        public ClientLocalException(string message) : base(message)
        {
        }
    }
}
