using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD
{
    public class Meeting
    {
        private string topic;
        private int minAttendees;
        private List<string> slots;
        private int coordinator;
        private List<int> invitees;
        //private List<Object> candidatos;  A tratar no join!!! e talvez por o coordenador por defeito
        
        public Meeting(string topic, int minAttendees, List<string> slots,List<int> invitees, int port)
        {
            this.topic = topic;
            this.minAttendees = minAttendees;
            this.slots = slots;
            this.coordinator = port;
            this.invitees = invitees;
        }
        public string getTopic()
        {
            return this.topic;
        }
        public string getSlotsData()
        {
            string slotsData = "";

            foreach (string s in this.slots)
            {
                slotsData += s;
                slotsData += "\n";
            }

            return slotsData;
        }

        public Boolean isInvited(int port)
        {
            if(this.invitees == null)
            {
                return true;
            }

            return this.invitees.Contains(port);
        }

    }
}
