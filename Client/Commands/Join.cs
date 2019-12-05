using MSDAD.Client.Commands;
using MSDAD.Client.Exceptions;
using MSDAD.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Client.Commands
{
    class Join : Command
    {
        string[] words;

        public Join(ref ClientLibrary client_library, string[] words) : base(ref client_library)
        {
            this.words = words;
        }

        public override object Execute()
        {

            int num_slots;
            string room, meeting_topic;

            List<string> slots = new List<string>();

            meeting_topic = this.words[1];
            
            try
            {
                num_slots = Int32.Parse(this.words[2]);
            }
            catch (FormatException e)
            {
                throw new ClientLocalException(ErrorCodes.INVALID_N_SLOTS);
            }

            if (num_slots < 1)
            {
                throw new ClientLocalException(ErrorCodes.INVALID_N_SLOTS);
            }

            for (int i = 3; i < (num_slots)+3; i++)
            {
                Console.WriteLine();
                room = this.words[i];

                if (slots.Contains(room))
                {
                    throw new ClientLocalException(ErrorCodes.DUPLICATED_SLOT);
                }
                
                slots.Add(room);
            }

            try
            {
                this.remote_server.Join(meeting_topic, slots, this.client_identifier, null, 0, null, Int32.MinValue);

            }
            catch (ServerCoreException sce)
            {
                Console.WriteLine(sce.Message);
            }
            catch (Exception exception) when (exception is System.Net.Sockets.SocketException || exception is System.IO.IOException)
            {
                this.remote_server = new ServerChange(ref base.client_library).Execute();
                if (this.remote_server != null)
                {
                    int n_replicas = this.remote_server.Hello(this.client_identifier, this.client_remoting, this.client_ip, this.client_port);
                    base.client_library.NReplicas = n_replicas;
                    this.remote_server.Join(meeting_topic, slots, this.client_identifier, null, 0, null, Int32.MinValue);
                }
                else
                {
                    throw new ClientLocalException("We cannot find anymore servers to connect to! Aborting...");
                }
            }

            return null;
        }
    }
}
