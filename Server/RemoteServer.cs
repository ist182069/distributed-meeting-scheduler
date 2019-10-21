using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD
{
    class RemoteServer : MarshalByRefObject, ServerInterface
    {
        public void Close(string meeting_topic)
        {
            throw new NotImplementedException();
        }

        public void Create(string meeting_topic)
        {
            throw new NotImplementedException();
        }

        public void Join(string meeting_topic)
        {
            throw new NotImplementedException();
        }

        public string List()
        {
            throw new NotImplementedException();
        }

        public void Wait(int milliseconds)
        {
            throw new NotImplementedException();
        }
    }
}
