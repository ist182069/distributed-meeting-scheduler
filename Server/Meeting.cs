using MSDAD.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD
{
    enum state { OPEN, SCHEDULED, CANCELED };

    public class Meeting
    {
        private string topic;
        private int minAttendees;
        private List<string> slots;
        private int coordinator;
        private List<int> invitees;
        private Dictionary<int, List<string>> candidates;
        private state state;
        private int version;

        public Meeting(string topic, int minAttendees, List<string> slots, List<int> invitees, int port)
        {
            this.topic = topic;
            this.minAttendees = minAttendees;
            this.slots = slots;
            this.coordinator = port;
            this.invitees = invitees;
            this.candidates = new Dictionary<int, List<string>>();
            this.state = state.OPEN;
            this.version = 1;
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
            if (state == state.CANCELED)
            {
                throw new ServerCommunicationException("This Meeting is already CANCELED");
            }
            lock (this)
            {
                this.candidates.Add(port, slots);
                this.version += 1;
            }
        }

        public void Schedule()
        {
            if (getNumberOfCandidates() < MinAttendees)
            {
                this.state = state.CANCELED;
            }
            this.state = state.SCHEDULED;

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

        public int MinAttendees
        {
            get
            {
                return this.minAttendees;
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
        public int getNumberOfCandidates()
        {
            return candidates.Count();
        }
        public int getVersion()
        {
            return this.version;
        }
        public List<string> getSlots()
        {
            return this.slots;
        }

    }
}
