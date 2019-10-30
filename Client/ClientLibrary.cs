using MSDAD.Client.Comunication;
using MSDAD.Client.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Client
{
    class ClientLibrary
    {
        int port;
        string ip;
        ClientCommunications communications;
        Command commandClass;

        private List<MeetingView> meetingViews = new List<MeetingView>();

        public ClientLibrary(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
            this.communications = new ClientCommunications(this, ip,  port); ;

            Console.Write("Starting client remoting service... ");
            communications.Start();
            Console.WriteLine("Success!");

            /*Console.Write("Getting the Remoting Communication class from the servers... ");
            sendComm.Start();
            Console.WriteLine("Success!");

            Console.Write("Initiating the handshake protocol... ");
            sendComm.Hello();
            Console.WriteLine("Success!");*/
        }

        public void AddMeetingView(MeetingView meetingView)
        {
            lock(this)
            {
                foreach (MeetingView mV in meetingViews)
                {
                    if (mV.GetTopic().Equals(meetingView.GetTopic()))
                        meetingViews.Remove(mV);
                    break;
                }

                this.meetingViews.Add(meetingView);
            }
        }

        public int GetPort()
        {
            return this.port;
        }

        public string GetIP()
        {
            return this.ip;
        }
        public List<MeetingView> GetMeetingViews()
        {
            lock (this)
            {
                return this.meetingViews;
            }   
        }

        private void SetPort(int port)
        {
            this.port = port;
        }

        private void SetMeetingViews(List<MeetingView> meetingViews)
        {
            lock (this)
            {
                this.meetingViews = meetingViews;
            }
        }      

        /*public void Ping()
        {
            this.sendComm.Ping();
        }

        public void Create()
        {
            MeetingView meetingView;

            this.commandClass = new Create();
            meetingView = (MeetingView) commandClass.Execute(this.sendComm, this.port);

            this.AddMeetingView(meetingView);
        }
        public void List()
        {            
            this.commandClass = new List();
            
            // TODO recebe o objecto do estado
            commandClass.Execute(this.sendComm, this.port);

            foreach(MeetingView meetingView in this.meetingViews)
            {
                // TODO fazer funcao privada local a esta classe para parsar o conteudo das meetingView
                Console.WriteLine(meetingView.GetTopic());
            }
        }
        public void Join()
        {
            string topic;

            this.commandClass = new Join();

            topic = (string) commandClass.Execute(this.sendComm, this.port);

            Console.WriteLine(topic);
        }
        public void AddMeetingView(MeetingView meetingView)
        {
            lock(this)
            {
                this.meetingViews.Add(meetingView);
            }
        }*/
    }
}
