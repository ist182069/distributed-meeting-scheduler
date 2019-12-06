using MSDAD.Client.Commands;
using MSDAD.Client.Exceptions;
using MSDAD.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
                while (true)
                {
                    try
                    {
                        this.remote_server.Create(meeting_topic, min_attendees, slots, null, this.client_identifier, null, 0, null, Int32.MinValue);
                        break;
                    }
                    catch (ServerCoreException sce)
                    {
                        Console.WriteLine(sce.Message);
                    }
                    catch (Exception exception) when (exception is System.Net.Sockets.SocketException || exception is System.IO.IOException)
                    {
                        this.remote_server = new ServerChange(ref base.client_library).Execute(); //returns remote_server or null;

                        if (this.remote_server != null)
                        {
                            try
                            {
                                int n_replicas = this.remote_server.Hello(this.client_identifier, this.client_remoting, this.client_ip, this.client_port);
                                base.client_library.NReplicas = n_replicas;
                            }
                            catch (System.Net.Sockets.SocketException)
                            {
                                Console.WriteLine("We cannot find anymore servers to connect to! Aborting...\r\n");
                                Console.Write("Crashing Client in...");
                                Thread.Sleep(1000);
                                Console.Write("3 ");
                                Thread.Sleep(1000);
                                Console.Write("2 ");
                                Thread.Sleep(1000);
                                Console.Write("1 ");
                                Thread.Sleep(1000);
                                Process.GetCurrentProcess().Kill();
                            }
                            
                            continue;
                        }
                        else
                        {
                            throw new ClientLocalException("We cannot find anymore servers to connect to! Aborting...");
                        }                        
                    }
                }
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

                    invitees.Add(invitee_address);
                }

                while (true)
                {
                    try
                    {
                        this.remote_server.Create(meeting_topic, min_attendees, slots, invitees, this.client_identifier, null, 0, null, Int32.MinValue);
                        break;
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
                            try
                            {
                                int n_replicas = this.remote_server.Hello(this.client_identifier, this.client_remoting, this.client_ip, this.client_port);
                                base.client_library.NReplicas = n_replicas;
                            }
                            catch (System.Net.Sockets.SocketException)
                            {
                                Console.WriteLine("We cannot find anymore servers to connect to! Aborting...");
                                CrashClientProcess();
                            }
                            continue;
                        }
                        else
                        {
                            Console.WriteLine("We cannot find anymore servers to connect to! Aborting...");
                            CrashClientProcess();
                        }
                    }
                }
            }
                
            return null;
        }
    }
}
