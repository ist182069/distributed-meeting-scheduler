using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD
{
    public interface ClientInterface
    {

    }

    public interface ServerInterface
    {
        String List();
        void Create(String meeting_topic);
        void Join(String meeting_topic);
        void Close(String meeting_topic);
        void Wait(int milliseconds);
    }
}
