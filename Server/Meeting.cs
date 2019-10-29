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
        private List<Tuple<Location, DateTime>> slots;
        private int coordinator;
        private List<int> invitees;
        private Dictionary<int, List<Tuple<Location, DateTime>>> candidates;
        private state state;
        private int version;

        public Meeting(string topic, int minAttendees, List<Tuple<Location,DateTime>> slots, List<int> invitees, int port)
        {
            this.topic = topic;
            this.minAttendees = minAttendees;
            this.slots = slots;
            this.coordinator = port;
            this.invitees = invitees;
            this.candidates = new Dictionary<int, List<Tuple<Location, DateTime>>>();
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

        public void Apply(List<Tuple<Location, DateTime>> slots, int port)
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

            foreach (Tuple<Location, DateTime> s in this.slots)
            {
                slotsData += s.Item1.Name + ", " + s.Item2.ToString();
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
            List<string> result = new List<String>();
            foreach (Tuple<Location, DateTime> t in slots)
            {
                string slot = "";
                slot += t.Item1.Name;
                slot += t.Item2.ToString();
                result.Add(slot);
            }

            return result;
        }
        public string getState()
        {
            return this.state.ToString();
        }

    }
}
