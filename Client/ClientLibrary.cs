using MSDAD.Client.Commands;
using MSDAD.Client.Comunication;
using MSDAD.Client.Commands.CLI;
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
        string ip, user, server;
        ClientCommunication communications;

        private List<MeetingView> meetingViews = new List<MeetingView>();

        public ClientLibrary(string user, string server, string ip, int port)
        {
            this.user = user;
            this.server = server;
            this.ip = ip;
            this.port = port;

            this.communications = new ClientCommunication(this, user, ip,  port); ;

            Console.Write("Starting client remoting service... ");
            communications.Start();
            Console.WriteLine("Success!");  
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

        public string GetUser()
        {
            return this.user;
        }
        public string GetServerUrl()
        {
            return this.server;
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
    }
}
