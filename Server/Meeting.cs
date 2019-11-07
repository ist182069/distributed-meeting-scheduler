using MSDAD.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Server
{
    enum state {OPEN, SCHEDULED, CANCELED};

    public class Meeting
    {
        private int min_attendees, version;
        private string coordinator, topic;
        private state state;
        private List<string> invitees;

        private Dictionary<Tuple<Location, DateTime>, List<string>> slots_clients_mapping;
        
        private List<string> going_clients;
        private string final_slot;

        public Meeting(string meeting_topic, int min_attendees, List<Tuple<Location,DateTime>> slots, List<string> invitees, string client_identifier)
        {
            this.topic = meeting_topic;
            this.min_attendees = min_attendees;            
            this.coordinator = client_identifier;      
            this.state = state.OPEN;
            this.version = 1;            
            this.InitInviteesList(invitees, client_identifier);
            this.InitVenuesDictionary(slots);
        }

        private void InitInviteesList(List<string> invitees, string client_identifier)
        {
            if (invitees != null)
            {
                this.invitees = invitees;
                this.invitees.Add(client_identifier);                   
            }
            else
            {
                this.invitees = null;
            }    
        }

        private void InitVenuesDictionary(List<Tuple<Location, DateTime>> slots)
        {
            List<string> candidates;

            this.slots_clients_mapping = new Dictionary<Tuple<Location, DateTime>, List<string>>();

            foreach (Tuple<Location,DateTime> tuple in slots)
            {
                candidates = new List<string>();
                this.slots_clients_mapping.Add(tuple, candidates);
            }
                
        }

        public void Apply(List<Tuple<Location, DateTime>> slots, string client_identifier)
        {

            if (state == state.CANCELED)
            {
                throw new ServerCoreException(ErrorCodes.MEETING_ALREADY_CANCELED);
            }
            else if (state == state.SCHEDULED)
            {
                throw new ServerCoreException(ErrorCodes.MEETING_ALREADY_SCHEDULED);
            }
            lock (this)
            {
                if (this.CheckClientIfInVenues(slots, client_identifier))
                {
                    throw new ServerCoreException(ErrorCodes.CLIENT_IS_ALREADY_CANDIDATE);
                }
                
                if (this.invitees!=null)
                {
                    if (this.invitees.Contains(client_identifier))
                    {
                        this.AddClientToVenues(slots, client_identifier);
                    }
                    else
                    {
                        throw new ServerCoreException(ErrorCodes.CLIENT_IS_NOT_INVITED);
                    }
                }
                else
                {
                    this.AddClientToVenues(slots, client_identifier);
                }                                    
                this.version += 1;
            }
        }

        private bool CheckClientIfInVenues(List<Tuple<Location, DateTime>> slots, string client_address)
        {
            bool result = false; 

            foreach(List<string> client_list in this.slots_clients_mapping.Values)
            {
                result = client_list.Contains(client_address);

                if(result)
                {
                    break;
                }
            }

            return result;
        }
        private void AddClientToVenues(List<Tuple<Location, DateTime>> slots, string client_address)
        {
            foreach (Tuple<Location, DateTime> tuple in slots)
            {
                if (this.slots_clients_mapping.ContainsKey(tuple))
                {                           
                    this.slots_clients_mapping[tuple].Add(client_address);
                }
                else
                {
                    throw new ServerCoreException(ErrorCodes.INVALID_SLOT);
                }
            }
        }
        

        public void Schedule(string client_identifier)
        {
            if (client_identifier != this.coordinator)
            {
                throw new ServerCoreException(ErrorCodes.NOT_COORDINATOR);
            }

            int client_count, going_people;

            int chosen_people = 0;
           
            DateTime dateTime, resultDateTime = new DateTime();
            Location location, resultLocation = null;
            Room resultRoom = null;

            Tuple<Location, DateTime> tuple;
            Dictionary<Location, Tuple<Room, DateTime, int>> proposedRoomsLocation = new Dictionary<Location, Tuple<Room, DateTime, int>>();

            lock(this)
            {
                foreach (KeyValuePair<Tuple<Location, DateTime>, List<string>> entry in this.slots_clients_mapping)
                {
                    client_count = entry.Value.Count;
                    tuple = entry.Key;

                    if (client_count >= min_attendees)
                    {
                        location = tuple.Item1;
                        dateTime = tuple.Item2;
                        
                        resultRoom = location.Select(dateTime, client_count, min_attendees);

                        if (resultRoom.Capacity>=client_count)
                        {
                            going_people = client_count;
                        }
                        else
                        {
                            going_people = resultRoom.Capacity;
                        }

                        Tuple<Room, DateTime, int> newTuple = new Tuple<Room, DateTime, int>(resultRoom, dateTime, going_people);
                        proposedRoomsLocation.Add(location, newTuple);
                    }
                }

                if(proposedRoomsLocation.Count == 0)
                {
                    this.state = state.CANCELED;
                    return;
                }

                resultRoom = null;

                foreach (KeyValuePair<Location, Tuple<Room, DateTime, int>> entry in proposedRoomsLocation)
                {
                    going_people = entry.Value.Item3;
                    int room_cap = entry.Value.Item1.Capacity;

                    if (resultRoom == null)
                    {
                        chosen_people = going_people;

                        resultLocation = entry.Key;
                        resultRoom = entry.Value.Item1;
                        resultDateTime = entry.Value.Item2;
                    }
                    else if(going_people > chosen_people)
                    {
                        chosen_people = going_people;

                        resultLocation = entry.Key;
                        resultRoom = entry.Value.Item1;
                        resultDateTime = entry.Value.Item2;
                    }
                    else if (going_people == chosen_people && resultRoom.Capacity > entry.Value.Item1.Capacity)
                    {
                        resultLocation = entry.Key;
                        resultRoom = entry.Value.Item1;
                        resultDateTime = entry.Value.Item2;
                    }
                }

                resultLocation.Pick(resultRoom, resultDateTime);
                this.final_slot = resultLocation.Name+" "+resultRoom.Identifier+" "+resultDateTime.ToString();
                List<string> going_clients = new List<string>();

                for(int i = 0; i<chosen_people; i++)
                {
                    going_clients.Add(this.slots_clients_mapping[new Tuple<Location, DateTime>(resultLocation, resultDateTime)][i]);
                }

                this.going_clients = going_clients;
                this.state = state.SCHEDULED;
            }
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
                return this.min_attendees;
            }
        }
            
        public int Version
        {
            get
            {
                return version;
            }
        }
       
        public string State
        {
            get
            {
                return this.state.ToString();
            }
        }  
        
        public List<string> GetInvitees()
        {
            return this.invitees;
        }
        
        public bool ClientConfirmed(string client_address)
        {
            if(going_clients==null)
            {
                return false;
            }
            return going_clients.Contains(client_address);
        }

        public string FinalSlot
        {
            get
            {
                return this.final_slot;
            }
        }

    }
}
