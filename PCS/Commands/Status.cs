using MSDAD.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;

namespace MSDAD.PCS.Commands
{
    class Status : Command
    {
        public delegate void StatusClientAsyncDelegate();
        public delegate void StatusServerAsyncDelegate();

        public static void StatusClientAsyncCallBack(IAsyncResult ar)
        {
            StatusClientAsyncDelegate del = (StatusClientAsyncDelegate)((AsyncResult)ar).AsyncDelegate;
            return;
        }

        public static void StatusServerAsyncCallBack(IAsyncResult ar)
        {
            StatusServerAsyncDelegate del = (StatusServerAsyncDelegate)((AsyncResult)ar).AsyncDelegate;
            return;
        }
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

                StatusClientAsyncDelegate RemoteDel = new StatusClientAsyncDelegate(remote_client.Status);
                AsyncCallback RemoteCallback = new AsyncCallback(Status.StatusClientAsyncCallBack);
                IAsyncResult RemAr = RemoteDel.BeginInvoke(RemoteCallback, null);
            }

            foreach (Tuple<string, Process> urlProcessTuple in server_dictionary.Values)
            {
                server_url = urlProcessTuple.Item1;

                remote_server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), server_url);

                StatusServerAsyncDelegate RemoteDel = new StatusServerAsyncDelegate(remote_server.Status);
                AsyncCallback RemoteCallback = new AsyncCallback(Status.StatusServerAsyncCallBack);
                IAsyncResult RemAr = RemoteDel.BeginInvoke(RemoteCallback, null);
            }

            return null;
        }
    }
}
