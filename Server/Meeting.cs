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

        private List<string> invitees;

        private Dictionary<Tuple<Location, DateTime>, List<string>> venuesClientMapping;

        public Meeting(string topic, int minAttendees, List<Tuple<Location,DateTime>> slots, List<string> invitees, string client_address)
        {
            this.topic = topic;
            this.minAttendees = minAttendees;            
            this.coordinator = client_address;      
            this.state = state.OPEN;
            this.version = 1;            
            this.InitInviteesList(invitees, client_address);
            this.InitVenuesDictionary(slots);
        }

        private void InitInviteesList(List<string> invitees, string client_address)
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

        private void InitVenuesDictionary(List<Tuple<Location, DateTime>> slots)
        {
            List<string> candidates;

            this.venuesClientMapping = new Dictionary<Tuple<Location, DateTime>, List<string>>();

            foreach (Tuple<Location,DateTime> tuple in slots)
            {
                candidates = new List<string>();
                this.venuesClientMapping.Add(tuple, candidates);
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
                        this.AddClientToVenues(slots, client_address);
                    }
                    else
                    {
                        throw new ServerCommunicationException("Apply(): Client:\"" + client_address + "\" is not invited to the said meeting.");
                    }
                }
                else
                {
                    this.AddClientToVenues(slots, client_address);
                } 
                    
                this.version += 1;
            }
        }

        private void AddClientToVenues(List<Tuple<Location, DateTime>> slots, string client_address)
        {
            string location_string, date_string;

            foreach (Tuple<Location, DateTime> tuple in slots)
            {
                if (this.venuesClientMapping.ContainsKey(tuple))
                {                    
                    this.venuesClientMapping[tuple].Add(client_address);
                }
                else
                {
                    location_string = tuple.Item1.Name;
                    date_string = tuple.Item2.ToString();

                    throw new ServerCommunicationException("ValidateSlots(): slot \"" + location_string + "," + date_string + "\" was not proposed by the coordinator");
                }
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
      
        public int GetNumberOfCandidates()
        {
            int count = 0;

            foreach (List<string> client_list in this.venuesClientMapping.Values)
            {
                foreach(string client_string in client_list)
                {
                    count++;
                }
            }

            return count;
        }
        public int GetVersion()
        {
            return this.version;
        }
       
        public string GetState()
        {
            return this.state.ToString();
        }    

    }
}
