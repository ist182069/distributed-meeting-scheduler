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
        private Dictionary<int, List<string>> candidates;
        private Boolean scheduled;

        public Meeting(string topic, int minAttendees, List<string> slots, List<int> invitees, int port)
        {
            this.topic = topic;
            this.minAttendees = minAttendees;
            this.slots = slots;
            this.coordinator = port;
            this.invitees = invitees;
            this.candidates = new Dictionary<int, List<string>>();
            this.scheduled = false;
        }


        public Boolean isInvited(int port)
        {
            if(this.invitees == null)
            {
                return true;
            }

            return this.invitees.Contains(port);
        }

        public Boolean isCandidate(int port)
        {
            return candidates.ContainsKey(port);
        }

        public void Apply(List<string> slots, int port)
        {
            lock (this)
            {
                this.candidates.Add(port, slots);
            }
        }

        public void Schedule()
        {
            scheduled = true;

            //TO DO logica do close
        }

        public string Topic
        {
            get
            {
                return this.topic;
            }
        }

        public int Coordinator
        {
            get
            {
                return this.coordinator;
            }
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
