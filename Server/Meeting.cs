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
        //private List<Object> candidatos;  A tratar no join!!!
        
        public Meeting(string topic, int minAttendees, List<string> slots)
        {
            this.topic = topic;
            this.minAttendees = minAttendees;
            this.slots = slots;
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

    }
}
