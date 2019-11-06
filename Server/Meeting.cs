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
        private int minAttendees, version;
        private string coordinator, topic;
        private state state;
        private List<string> invitees;

        private Dictionary<Tuple<Location, DateTime>, List<string>> venuesClientMapping;
        
        private List<string> goingClients;
        private string finalSlot;

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
                throw new ServerCoreException(ErrorCodes.MEETING_ALREADY_CANCELED);
            }
            else if (state == state.SCHEDULED)
            {
                throw new ServerCoreException(ErrorCodes.MEETING_ALREADY_SCHEDULED);
            }
            lock (this)
            {
                if (this.CheckClientIfInVenues(slots, client_address))
                {
                    throw new ServerCoreException(ErrorCodes.CLIENT_IS_ALREADY_CANDIDATE);
                }
                
                if (this.invitees!=null)
                {
                    if (this.invitees.Contains(client_address))
                    {
                        this.AddClientToVenues(slots, client_address);
                    }
                    else
                    {
                        throw new ServerCoreException(ErrorCodes.CLIENT_IS_NOT_INVITED);
                    }
                }
                else
                {
                    this.AddClientToVenues(slots, client_address);
                }                                    
                this.version += 1;
            }
        }

        private bool CheckClientIfInVenues(List<Tuple<Location, DateTime>> slots, string client_address)
        {
            bool result = false; 

            foreach(List<string> client_list in this.venuesClientMapping.Values)
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
            bool client_not_added_flag = true;

            foreach (Tuple<Location, DateTime> tuple in slots)
            {
                if (this.venuesClientMapping.ContainsKey(tuple))
                {                           
                    this.venuesClientMapping[tuple].Add(client_address);
                    client_not_added_flag = false;
                }
                else
                {
                    throw new ServerCoreException(ErrorCodes.ONE_INVALID_SLOT);
                }
            }
            
            if (client_not_added_flag)
            {
                throw new ServerCoreException(ErrorCodes.ALL_INVALID_SLOTS);
            }
        }
        

        public void Schedule(string client_address)
        {
            if (client_address != this.coordinator)
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
                foreach (KeyValuePair<Tuple<Location, DateTime>, List<string>> entry in this.venuesClientMapping)
                {
                    client_count = entry.Value.Count;
                    tuple = entry.Key;

                    if (client_count >= MinAttendees)
                    {
                        location = tuple.Item1;
                        dateTime = tuple.Item2;
                        
                        resultRoom = location.Select(dateTime, client_count, MinAttendees);

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
                this.finalSlot = resultLocation.Name+" "+resultRoom.Identifier+" "+resultDateTime.ToString();
                List<string> goingClients = new List<string>();

                for(int i = 0; i<chosen_people; i++)
                {
                    goingClients.Add(this.venuesClientMapping[new Tuple<Location, DateTime>(resultLocation, resultDateTime)][i]);
                }

                this.goingClients = goingClients;
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
                return this.minAttendees;
            }
        }
            
        public int GetVersion()
        {
            return this.version;
        }
       
        public string GetState()
        {
            return this.state.ToString();
        }  
        
        public List<string> GetInvitees()
        {
            return this.invitees;
        }
        
        public bool ClientConfirmed(string client_addr)
        {
            if(goingClients==null)
            {
                return false;
            }
            return goingClients.Contains(client_addr);
        }

        public string GetFinalSlot()
        {
            return this.finalSlot;
        }

    }
}
