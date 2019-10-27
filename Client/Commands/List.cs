using MSDAD.Client.Comunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Client.Commands
{
    class List : Command
    {
        public List(ref ClientLibrary clientLibrary) : base(ref clientLibrary)
        {

        }
        public override object Execute()
        {
            List<MeetingView> meetingViews;

            meetingViews = this.clientLibrary.GetMeetingViews();

            foreach (MeetingView meetingView in meetingViews)
            {
                // TODO fazer funcao privada local a esta classe para parsar o conteudo das meetingView
                Console.WriteLine(meetingView.GetTopic());
            }

            this.server.List(this.port);

            // TODO ira processar o estado que ira receber do servidor

            return null;
        }
    }
}
