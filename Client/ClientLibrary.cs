using MSDAD.Client.Comunication;
using MSDAD.Client.Commands;
using MSDAD.Client.Library;
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
        ClientReceiveComm receiveComm;
        ClientSendComm sendComm;
        Command commandClass;

        private List<MeetingView> meetingViews = new List<MeetingView>();

        public ClientLibrary(int port)
        {
            this.port = port;
            this.receiveComm = new ClientReceiveComm(this, port);
            this.sendComm = new ClientSendComm(port);

            Console.Write("Starting client remoting service... ");
            receiveComm.Start();
            Console.WriteLine("Success!");

            Console.Write("Getting the Remoting Communication class from the servers... ");
            sendComm.Start();
            Console.WriteLine("Success!");

            Console.Write("Initiating the handshake protocol... ");
            sendComm.Hello();
            Console.WriteLine("Success!");
        }

        public void Ping()
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
        }
    }
}
