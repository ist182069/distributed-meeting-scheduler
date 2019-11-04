using MSDAD.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD
{
    enum state {OPEN, SCHEDULED, CANCELED};

    public class Meeting
    {
        private static readonly DateTime ERRONEOUS_DATE = new DateTime(1900, 1, 31);

        private int minAttendees, version;
        private string coordinator, topic;

        private state state;

        private List<string> candidates, invitees;

        Tuple<Location, string, DateTime> chosenVenue;

        // private Dictionary<Tuple<Location, DateTime>, List<string>> candidates;

        private List<Tuple<Location, DateTime>> proposedVenues;

        public Meeting(string topic, int minAttendees, List<Tuple<Location,DateTime>> slots, List<string> invitees, string client_address)
        {
            this.topic = topic;
            this.minAttendees = minAttendees;            
            this.coordinator = client_address;
            this.candidates = = new List<string>();
            this.proposedVenues = new List<Tuple<Location, DateTime>>(slots);          
            this.state = state.OPEN;
            this.version = 1;            
            this.InitClientList(invitees, client_address);
        }

        private void InitClientList(List<string> invitees, string client_address)
        {
            if (invitees != null)
            {
                this.invitees = invitees;
                this.invitees.Add(client_address);                   
            }
            else
            {
                this.invitees = null;
            }    
        }
        public void Apply(List<Tuple<Location, DateTime>> slots, string client_address)
        {

            if (state == state.CANCELED)
            {
                throw new ServerCommunicationException("This Meeting is already CANCELED.");
            }
            lock (this)
            {
                if(this.invitees!=null)
                {
                    if (this.invitees.Contains(client_address))
                    {
                        this.candidates.Add(client_address);
                    }
                    else
                    {
                        throw new ServerCommunicationException("Apply(): Client:\"" + client_address + "\" is not invited to the said meeting.");
                    }
                }
                else
                {
                    this.candidates.Add(client_address);
                } 
    
                

                this.proposedVenues.Concat(slots);
                this.version += 1;
            }
        }

        public void Schedule()
        {
            if(GetNumberOfCandidates() < MinAttendees)
            {
                this.state = state.CANCELED;
            }

            this.state = state.SCHEDULED;

            // TODO logica do close
        }

        public string Topic
        {
            get
            {
                return this.topic;
            }
        }

        public string Coordinator
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

        public string GetSlotsData()
        {
            string slotsData = "";

            foreach (Tuple<Location, DateTime> s in this.proposedVenues)
            {
                slotsData += s.Item1.Name + ", " + s.Item2.ToString();
                slotsData += "\n";
            }

            return slotsData;
        }
        public int GetNumberOfCandidates()
        {
            return this.candidates.Count();
        }
        public int GetVersion()
        {
            return this.version;
        }
        public List<string> GetSlots()
        {
            List<string> result = new List<String>();
            foreach (Tuple<Location, DateTime> t in this.proposedVenues)
            {
                string slot = "";
                slot += t.Item1.Name;
                slot += t.Item2.ToString();
                result.Add(slot);
            }

            return result;
        }
        public string GetState()
        {
            return this.state.ToString();
        }

        public List<string> GetClients()
        {
            return this.candidates;
        }

    }
}
