using MSDAD.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MSDAD.PCS.Commands
{
    class Status : Command
    {
        public Status(ref PCSLibrary pcsLibrary) : base(ref pcsLibrary)
        {
        }
        public override object Execute()
        {
            string client_url, server_url;

            ClientInterface remote_client;
            ServerInterface remote_server;

            Dictionary<string, Tuple<string, Process>> client_dictionary;
            Dictionary<string, Tuple<string, Process>> server_dictionary;

            client_dictionary = base.pcsLibrary.GetClientDictionary();
            server_dictionary = base.pcsLibrary.GetServerDictionary();

            foreach (Tuple<string, Process> urlProcessTuple in client_dictionary.Values)
            {
                client_url = urlProcessTuple.Item1;
                remote_client = (ClientInterface)Activator.GetObject(typeof(ClientInterface), client_url);                
            }

            foreach (Tuple<string, Process> urlProcessTuple in server_dictionary.Values)
            {
                server_url = urlProcessTuple.Item1;
                remote_server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), server_url);
                remote_server.Status();
            }

            return null;
        }
    }
}
