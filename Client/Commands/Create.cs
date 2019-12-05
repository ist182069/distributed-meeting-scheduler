using MSDAD.Client.Commands;
using MSDAD.Client.Exceptions;
using MSDAD.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Client.Commands
{
    class Create : Command
    {
        string[] words;

        public Create(ref ClientLibrary client_library, string[] words) : base(ref client_library)
        {
            this.words = words;
        }

        public override object Execute()
        {

            int min_attendees, num_slots, num_invitees;
            string invitee_address, room, meeting_topic;

            MeetingView meeting_view;
            List<string> slots = new List<string>(), invitees;

            meeting_topic = this.words[1];

            try
            {
                min_attendees = Int32.Parse(this.words[2]);
            }
            catch (FormatException e)
            {
                throw new ClientLocalException(ErrorCodes.INVALID_MIN_ATTENDES);
            }

            if (min_attendees < 1)
            {
                throw new ClientLocalException(ErrorCodes.INVALID_MIN_ATTENDES);
            }

            try
            {
                num_slots = Int32.Parse(this.words[3]);
            }
            catch (FormatException e)
            {
                throw new ClientLocalException(ErrorCodes.INVALID_N_SLOTS);
            }

            if (num_slots < 1)
            {
                throw new ClientLocalException(ErrorCodes.INVALID_N_SLOTS);
            }

            try
            {
                num_invitees = Int32.Parse(this.words[4]);
            }
            catch (FormatException e)
            {
                throw new ClientLocalException(ErrorCodes.INVALID_N_INVITEES);
            }

            if (num_invitees < 0)
            {
                throw new ClientLocalException(ErrorCodes.INVALID_N_INVITEES);
            }                        

            for(int i = 5; i <= (num_slots+4); i++)
            {
                room = this.words[i];
                
                if (slots.Contains(room))
                {
                    throw new ClientLocalException(ErrorCodes.DUPLICATED_SLOT);
                }
                slots.Add(room);                
            }

            if(num_invitees==0)
            {
                try
                {
                    this.remote_server.Create(meeting_topic, min_attendees, slots, null, this.client_identifier, null, 0, null, Int32.MinValue);
                } catch(ServerCoreException sce)
                {
                    Console.WriteLine(sce.Message);
                }
                catch (Exception exception) when (exception is System.Net.Sockets.SocketException || exception is System.IO.IOException)
                {
                    this.remote_server = new ServerChange(ref base.client_library).Execute();

                    if (this.remote_server!=null)
                    {
                        int n_replicas = this.remote_server.Hello(this.client_identifier, this.client_remoting, this.client_ip, this.client_port);
                        base.client_library.NReplicas = n_replicas;
                        this.remote_server.Create(meeting_topic, min_attendees, slots, null, this.client_identifier, null, 0, null, Int32.MinValue);
                    }
                    else
                    {
                        throw new ClientLocalException("We cannot find anymore servers to connect to! Aborting...");
                    }
                }

                meeting_view = new MeetingView(meeting_topic, 1, "OPEN", null);
                this.client_library.AddMeetingView(meeting_view);
            }
            else
            {
                invitees = new List<string>();
                for (int i = (num_slots + 4) + 1 ; i < this.words.Length; i++)
                {
                    invitee_address = this.words[i];

                    if (invitees.Contains(invitee_address))
                    {
                        throw new ClientLocalException(ErrorCodes.DUPLICATED_SLOT);
                    }

                    if (invitee_address == this.client_address)
                    {
                        meeting_view = new MeetingView(meeting_topic, 1, "OPEN", null);
                        this.client_library.AddMeetingView(meeting_view);
                    }

                    invitees.Add(invitee_address);
                }

                try
                {
                    this.remote_server.Create(meeting_topic, min_attendees, slots, invitees, this.client_identifier, null, 0, null, Int32.MinValue);
                } catch(ServerCoreException sce)
                {
                    Console.WriteLine(sce.Message);
                } catch(System.Net.Sockets.SocketException se)
                {
                    this.remote_server = new ServerChange(ref base.client_library).Execute();
                    if (this.remote_server != null)
                    {
                        int n_replicas = this.remote_server.Hello(this.client_identifier, this.client_remoting, this.client_ip, this.client_port);
                        base.client_library.NReplicas = n_replicas;
                        this.remote_server.Create(meeting_topic, min_attendees, slots, invitees, this.client_identifier, null, 0, null, Int32.MinValue);
                    }
                    else
                    {
                        throw new ClientLocalException("We cannot find anymore servers to connect to! Aborting...");
                    }
                }
                
            }
                
            return null;
        }
    }
}
