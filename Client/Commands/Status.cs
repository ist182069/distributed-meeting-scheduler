using MSDAD.Client;
using MSDAD.Client.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Client.Commands
{
    class Status : Command
    {
        public Status(ref ClientLibrary client_library) : base(ref client_library)
        {

        }
        public override object Execute()
        {
            int port;
            string client_identifier, client_ip, client_url, client_remoting, server_url;
            List<MeetingView> meetingViews;

            client_identifier = base.client_library.ClientIdentifier;
            client_ip = base.client_library.ClientIP;
            port = base.client_library.ClientPort;
            client_remoting = base.client_library.ClientRemoting;
            client_url = ClientUtils.AssembleRemotingURL(client_ip, port, client_remoting);
            server_url = base.client_library.ServerURL;
            meetingViews = base.client_library.GetMeetingViews();

            Console.WriteLine("client id: " + client_identifier);
            Console.WriteLine("remoting url: " + client_url);
            Console.WriteLine("server url: " + server_url);

            Console.WriteLine("Meeting Views: ");
            foreach (MeetingView meetingView in meetingViews)
            {
                Console.WriteLine("Topic: " + meetingView.MeetingTopic);
                Console.WriteLine("State: " + meetingView.MeetingState);                
                Console.WriteLine("Version: " + meetingView.MeetingVersion);
                Console.WriteLine("Info: " + meetingView.MeetingInfo);
            }


            return null;
        }
    }
}
