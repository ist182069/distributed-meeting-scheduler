using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Remoting;

namespace MSDAD
{
    namespace Library
    {
        [Serializable]
        public class ServerCommunicationException : RemotingException, ISerializable
        {
            private string message;

            public ServerCommunicationException()
            {
                this.message = String.Empty;
            }

            public ServerCommunicationException(string message)
            {
                this.message = message;
            }

            public ServerCommunicationException(SerializationInfo info, StreamingContext context)
            {
                this.message = (string)info.GetValue("message", typeof(string));
            }

            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("message", this.message);
            }

            public override string Message
            {
                get
                {
                    return this.message;
                }
            }
        }
    }
}



